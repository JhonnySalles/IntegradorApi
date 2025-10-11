using IntegradorApi.Data;
using IntegradorApi.Data.Core;
using IntegradorApi.Data.Enums;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Services;
using IntegradorApi.Services;
using IntegradorApi.Sync.Services;
using IntegradorApi.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MySqlConnector;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WinRT.Interop;


namespace IntegradorApi {
  /// <summary>
  /// An empty window that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class IntegradorApi : Window {

    private readonly SettingsService _settingsService;
    private System.Threading.Timer _syncTimer;
    private AppWindow _appWindow;

    private readonly DatabaseService _databaseService;
    private Connection? _connectEdit = null;
    private ObservableCollection<Connection> SourceConnections { get; set; } = new();
    private ObservableCollection<ConnectionStatusViewModel> StatusIcons { get; set; } = new();


    private readonly TimeSpan _initialSyncDelay = TimeSpan.FromMinutes(10);
    private readonly TimeSpan _recurringSyncInterval = TimeSpan.FromHours(3);

    public IntegradorApi() {
      InitializeComponent();
      SystemBackdrop = new MicaBackdrop();

      _settingsService = new SettingsService();
      ReadConfiguration();
      LoadSyncButtonState();

      var dbConfig = AppConfig.Configuration.GetSection("ConnectionStrings:LocalDatabase");
      string connectionString = $"Server={dbConfig["Address"]};" +
                                $"Port={dbConfig["Port"]};" +
                                $"Database={GlobalConstants.DatabaseName};" +
                                $"Uid={dbConfig["User"]};" +
                                $"Pwd={dbConfig["Password"]};";

      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
      optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
      _databaseService = new DatabaseService(optionsBuilder.Options, Log.Logger);

      var hwnd = WindowNative.GetWindowHandle(this);
      var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
      _appWindow = AppWindow.GetFromWindowId(windowId);

      TrayMenuFlyout.XamlRoot = this.Content.XamlRoot;
      MyTaskbarIcon.DataContext = this;

      LogEventBus.OnLogReceived += AppendLogToUI;
      StatusConectionsItemsControl.ItemsSource = StatusIcons;
      SourceDataGrid.ItemsSource = SourceConnections;
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs e) {
      await ApplyMigrationsAsync();
      await LoadSourceDataGridAsync();
    }

    private void ButtonClick_ToggleSyncronize(object sender, RoutedEventArgs e) {
      bool currentState = _settingsService.GetSincronizacaoStatus();
      _settingsService.SetSincronizacaoStatus(!currentState);
      LoadSyncButtonState();
    }

    private void Pressed_StatusIcon(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) {
      if (sender is FrameworkElement element && element.DataContext is ConnectionStatusViewModel statusVm)
        TestConnectionStatus(statusVm);
    }

    private void ButtonClick_ClearLog(object sender, RoutedEventArgs e) {
      LogTextBlock.Text = string.Empty;
    }

    private void ButtonClick_MinimizeToTray(object sender, RoutedEventArgs e) {
      this.Hide();
      MyTaskbarIcon.Visibility = Visibility.Visible;
    }

    private void Click_RestoreWindow(object sender, RoutedEventArgs e) {
      Serilog.Log.Information("Comando Restaurar Janela chamado.");
      this.Show();
      MyTaskbarIcon.Visibility = Visibility.Collapsed;
    }

    private void Click_ExitApp(object sender, RoutedEventArgs e) {
      Serilog.Log.Information("Comando Fechar Aplicação chamado.");
      Application.Current.Exit();
    }

    private void Click_CancelConnection(object sender, RoutedEventArgs e) {
      ReadConfiguration();
    }

    private async void Click_TestConnection(object sender, RoutedEventArgs e) {
      var connectionStringBuilder = new MySqlConnectionStringBuilder {
        Server = TxtDataBaseAddress.Text,
        Port = Convert.ToUInt32(TxtDataBasePort.Text),
        UserID = TxtDataBaseUser.Text,
        Password = PswDataBasePassword.Password,
        Database = GlobalConstants.DatabaseName,
        ConnectionTimeout = 5
      };

      // Tenta abrir e fechar a conexão
      MySqlConnection connection = null;
      try {
        connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();
        await ShowMessageDialog("Sucesso!", "A conexão com o banco de dados foi estabelecida com sucesso.");
      } catch (Exception ex) {
        await ShowMessageDialog("Falha na Conexão", $"Não foi possível conectar ao banco de dados.\n\nErro: {ex.Message}");
      } finally {
        if (connection?.State == System.Data.ConnectionState.Open) {
          await connection.CloseAsync();
        }
      }
    }

    private async void Click_SaveConnection(object sender, RoutedEventArgs e) {
      try {
        _settingsService.SaveConnectionSettings(
            TxtDataBaseDescription.Text,
            TxtDataBaseAddress.Text,
            TxtDataBasePort.Text,
            TxtDataBaseUser.Text,
            PswDataBasePassword.Password
        );

        await ApplyMigrationsAsync();
        await LoadSourceDataGridAsync();
        await ShowMessageDialog("Sucesso", "As configurações de conexão foram salvas com sucesso!");
      } catch (Exception ex) {
        await ShowMessageDialog("Erro ao Salvar", $"Ocorreu um erro ao salvar as configurações: {ex.Message}");
      }
    }

    private async void Click_AddSource(object sender, RoutedEventArgs e) {
      if (SourceTypeComboBox.SelectedItem == null || SourceDataComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(SourceAddressTextBox.Text)) {
        await ShowMessageDialog("Dados Incompletos", "Por favor, selecione uma conexão, fonte e preencha o endereço (URL).");
        return;
      }

      if (_connectEdit != null) {
        _connectEdit.TypeConnection = (ConnectionType)SourceTypeComboBox.SelectedIndex;
        _connectEdit.TypeDataSource = (DataSourceType)SourceDataComboBox.SelectedIndex;
        _connectEdit.Description = SourceDescriptionTextBox.Text;
        _connectEdit.Address = SourceAddressTextBox.Text;
        _connectEdit.User = SourceUserTextBox.Text;
        _connectEdit.Password = SourcePasswordTextBox.Text;
        _connectEdit.Optional = SourceOptionalTextBox.Text;
        _connectEdit.Enabled = SourceEnabledCheckBox.IsChecked ?? false;
        _connectEdit.Delete = SourceDeleteCheckBox.IsChecked ?? false;
        _connectEdit.TypeIntegration = (IntegrationType)SourceTypeComboBox.SelectedIndex;

        await _databaseService.UpdateConnectionAsync(_connectEdit);
      } else {
        var newConnection = new Connection {
          TypeConnection = (ConnectionType)SourceTypeComboBox.SelectedIndex,
          TypeDataSource = (DataSourceType)SourceDataComboBox.SelectedIndex,
          Description = SourceDescriptionTextBox.Text,
          Address = SourceAddressTextBox.Text,
          User = SourceUserTextBox.Text,
          Password = SourcePasswordTextBox.Text,
          Optional = SourceOptionalTextBox.Text,
          Enabled = SourceEnabledCheckBox.IsChecked ?? false,
          Delete = SourceDeleteCheckBox.IsChecked ?? false,
          TypeIntegration = (IntegrationType)SourceDataComboBox.SelectedIndex,
        };

        await _databaseService.AddConnectionAsync(newConnection);
        SourceConnections.Add(newConnection);
      }

      RefreshStatusIcons();
      ClearSourceForm();
    }

    private async void Click_DeleteSource(object sender, RoutedEventArgs e) {
      var selectedConnection = SourceDataGrid.SelectedItem as Connection;
      if (selectedConnection == null) {
        await ShowMessageDialog("Nenhum item selecionado", "Por favor, selecione uma conexão na grid para excluir.");
        return;
      }

      await _databaseService.DeleteConnectionAsync(selectedConnection);
      SourceConnections.Remove(selectedConnection);
      RefreshStatusIcons();
    }

    private async void Click_SourceEnabled(object sender, RoutedEventArgs e) {
      var checkBox = sender as CheckBox;
      var connectionToUpdate = checkBox?.DataContext as Connection;
      if (connectionToUpdate != null) {
        await _databaseService.UpdateConnectionAsync(connectionToUpdate);
        RefreshStatusIcons();
      }
    }

    private async void Click_TestGridConnection(object sender, RoutedEventArgs e) {
      if (sender is FrameworkElement element && element.DataContext is Connection connection)
        await TestSingleConnection(connection);

    }

    private async Task ShowMessageDialog(string title, string content) {
      ContentDialog dialog = new ContentDialog {
        Title = title,
        Content = content,
        CloseButtonText = "Ok",
        XamlRoot = this.Content.XamlRoot
      };
      await dialog.ShowAsync();
    }
    private void Hide() {
      _appWindow.Hide();
    }

    private void Show() {
      _appWindow.Show();
    }

    private void DoubleTapped_SourceDataGrid(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e) {
      var selectedConnection = SourceDataGrid.SelectedItem as Connection;
      if (selectedConnection == null)
        return;


      _connectEdit = selectedConnection;
      SourceDescriptionTextBox.Text = _connectEdit.Description;
      SourceAddressTextBox.Text = _connectEdit.Address;
      SourceUserTextBox.Text = _connectEdit.User;
      SourcePasswordTextBox.Text = _connectEdit.Password;
      SourceOptionalTextBox.Text = _connectEdit.Optional;
      SourceEnabledCheckBox.IsChecked = _connectEdit.Enabled;
      SourceDeleteCheckBox.IsChecked = _connectEdit.Delete;
      SourceTypeComboBox.SelectedIndex = (int)_connectEdit.TypeIntegration;
      SourceDataComboBox.SelectedIndex = (int)_connectEdit.TypeDataSource;
      SourceTypeComboBox.SelectedIndex = (int)_connectEdit.TypeConnection;

      AddingSourceButtons.Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new SymbolIcon(Symbol.Save), new TextBlock { Text = "Atualizar" } } };
    }

    private void ClearSourceForm() {
      _connectEdit = null;
      SourceDescriptionTextBox.Text = string.Empty;
      SourceAddressTextBox.Text = string.Empty;
      SourceUserTextBox.Text = string.Empty;
      SourcePasswordTextBox.Text = string.Empty;
      SourceOptionalTextBox.Text = string.Empty;
      SourceEnabledCheckBox.IsChecked = true;
      SourceDeleteCheckBox.IsChecked = false;
      AddingSourceButtons.Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new SymbolIcon(Symbol.Add), new TextBlock { Text = "Adicionar" } } };
    }

    private void RefreshStatusIcons() {
      StatusIcons.Clear();
      foreach (var connection in SourceConnections.Where(c => c.Enabled))
        StatusIcons.Add(new ConnectionStatusViewModel(connection));
    }

    private async void TestConnectionStatus(ConnectionStatusViewModel statusVm) {
      statusVm.StatusBrush = new SolidColorBrush(Colors.Yellow);

      var connStringBuilder = new MySqlConnectionStringBuilder {
        Server = statusVm.Connection.Address,
        Port = Convert.ToUInt32(TxtDataBasePort.Text),
        UserID = statusVm.Connection.User,
        Password = statusVm.Connection.Password,
        Database = GlobalConstants.DatabaseName,
        ConnectionTimeout = 5
      };

      try {
        await using var connection = new MySqlConnection(connStringBuilder.ConnectionString);
        await connection.OpenAsync();
        statusVm.StatusBrush = new SolidColorBrush(Colors.Green);
      } catch (Exception) {
        statusVm.StatusBrush = new SolidColorBrush(Colors.Red);
      }
    }

    private async Task TestSingleConnection(Connection connection) {
      var connStringBuilder = new MySqlConnectionStringBuilder {
        Server = connection.Address,
        Port = Convert.ToUInt32(TxtDataBasePort.Text),
        UserID = connection.User,
        Password = connection.Password,
        Database = GlobalConstants.DatabaseName,
        ConnectionTimeout = 5
      };

      try {
        await using var mysqlConn = new MySqlConnection(connStringBuilder.ConnectionString);
        await mysqlConn.OpenAsync();
        await ShowMessageDialog("Sucesso!", $"A conexão '{connection.Description}' foi estabelecida com sucesso.");
      } catch (Exception ex) {
        await ShowMessageDialog("Falha na Conexão", $"Não foi possível conectar em '{connection.Description}'.\n\nErro: {ex.Message}");
      }
    }

    private void AppendLogToUI(string message) {
      DispatcherQueue.TryEnqueue(() => {
        LogTextBlock.Text += message;
      });
    }

    private void ReadConfiguration() {
      TxtDataBaseDescription.Text = AppConfig.Configuration.GetConnectionString("LocalDatabase:Description");
      TxtDataBaseAddress.Text = AppConfig.Configuration.GetConnectionString("LocalDatabase:Address");
      TxtDataBasePort.Text = AppConfig.Configuration.GetConnectionString("LocalDatabase:Port");
      TxtDataBaseUser.Text = AppConfig.Configuration.GetConnectionString("LocalDatabase:User");
      PswDataBasePassword.Password = AppConfig.Configuration.GetConnectionString("LocalDatabase:Password");
    }

    private void LoadSyncButtonState() {
      bool isSyncEnabled = _settingsService.GetSincronizacaoStatus();

      var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };

      if (isSyncEnabled) {
        stackPanel.Children.Add(new SymbolIcon(Symbol.Pause));
        stackPanel.Children.Add(new TextBlock { Text = "Pausar Sincronizações" });
        BtnToggleSync.Content = stackPanel;
        StartTimer(comDelayInicial: true);
      } else {
        stackPanel.Children.Add(new SymbolIcon(Symbol.Play));
        stackPanel.Children.Add(new TextBlock { Text = "Iniciar Sincronizações" });
        BtnToggleSync.Content = stackPanel;
        StopTimer();
      }
    }

    private async Task ApplyMigrationsAsync() {
      Log.Information("Verificando e aplicando migrações do banco de dados...");
      try {
        await _databaseService.CreateDbContext().Database.MigrateAsync();
        Log.Information("Banco de dados está atualizado.");
      } catch (System.Exception ex) {
        Log.Fatal(ex, "Falha ao aplicar migrações do banco de dados. Verifique as configurações de conexão.");
        await ShowMessageDialog("Erro de Conexão", $"Não foi possível conectar ou atualizar o banco de dados. Por favor, verifique os dados na aba 'Conexões'.\n\nErro: {ex.Message}");
      }
    }

    private async Task LoadSourceDataGridAsync() {
      Log.Information("Carregando os dados.");
      try {
        var connections = await _databaseService.GetConnectionsAsync();
        DispatcherQueue.TryEnqueue(() => {
          SourceConnections.Clear();
          foreach (var conn in connections)
            SourceConnections.Add(conn);

          RefreshStatusIcons();
        });
      } catch (Exception ex) {
        DispatcherQueue.TryEnqueue(async () => {
          await ShowMessageDialog("Erro ao Carregar", $"Não foi possível carregar as fontes de dados: {ex.Message}");
        });
      }
    }

    private void StartTimer(bool comDelayInicial) {
      _syncTimer?.Dispose();

      TimeSpan dueTime = comDelayInicial ? _initialSyncDelay : TimeSpan.Zero;
      TimeSpan period = _recurringSyncInterval;

      _syncTimer = new System.Threading.Timer(RunSincronization, null, dueTime, period);

      Serilog.Log.Information("Timer de sincronização iniciado. Próxima execução em {DueTime}.", dueTime);
      DispatcherQueue.TryEnqueue(() => {
        LogTextBlock.Text += $"{DateTime.Now:G}: Timer de sincronização iniciado. Próxima execução em {dueTime}.\n";
      });
    }

    private void StopTimer() {
      _syncTimer?.Dispose();
      _syncTimer = null; // Anula a referência

      Serilog.Log.Information("Timer de sincronização parado.");
      DispatcherQueue.TryEnqueue(() => {
        LogTextBlock.Text += $"{DateTime.Now:G}: Timer de sincronização parado.\n";
      });
    }

    private void SincronizeManual() {
      Serilog.Log.Information("Sincronização manual acionada.");
      Task.Run(() => RunSincronization(null));

      if (_settingsService.GetSincronizacaoStatus()) {
        _syncTimer?.Change(_recurringSyncInterval, _recurringSyncInterval);
        Serilog.Log.Information("Timer resetado. Próxima execução automática em {Intervalo}", _recurringSyncInterval);
      }
    }


    // A lógica para os ícones de status seria implementada nos métodos
    // que são chamados pelos botões "Testar Conexões" ou pelo próprio processo de sincronização.
    // Exemplo:
    private void UpdateConnectionStatusIcons() {
      // Limpa os ícones atuais
      StatusConectionsItemsControl.Items.Clear();

      // Supondo que você tenha uma lista de conexões (ex: List<Connection> conexoes)
      // foreach (var conexao in conexoes)
      // {
      //     var icon = new FontIcon
      //     {
      //         FontFamily = new FontFamily("Segoe MDL2 Assets"),
      //         Glyph = "\uE753", // Ícone de "Plug"
      //         FontSize = 24
      //     };
      //
      //     // Define a cor baseada no status (lógica a ser implementada)
      //     // icon.Foreground = new SolidColorBrush(Colors.Green); // Sucesso
      //     // icon.Foreground = new SolidColorBrush(Colors.Red);   // Erro
      //     // icon.Foreground = new SolidColorBrush(Colors.Yellow);// Em processo
      //
      //     ToolTipService.SetToolTip(icon, new ToolTip { Content = conexao.Descricao });
      //
      //     StatusConectionsItemsControl.Items.Add(icon);
      // }
    }

    private async void RunSincronization(object state) {
      Serilog.Log.Information("Iniciando rotina de sincronização...");
      DispatcherQueue.TryEnqueue(() => {
        LogTextBlock.Text += $"{DateTime.Now:G}: Iniciando rotina de sincronização...\n";
      });

      // ===================================================================

      try {
        var orchestrator = new SyncOrchestrator(_databaseService, Log.Logger);
        await orchestrator.RunAllActiveSyncsAsync();
      } catch (Exception ex) {
        Log.Error(ex, "Erro não tratado no processo de orquestração.");
      }

      // ===================================================================
      Serilog.Log.Information("Rotina de sincronização concluída.");
      DispatcherQueue.TryEnqueue(() => {
        LogTextBlock.Text += $"{DateTime.Now:G}: Rotina de sincronização concluída.\n";
      });
    }
  }
}
