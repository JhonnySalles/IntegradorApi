using IntegradorApi.Core;
using IntegradorApi.Data;
using IntegradorApi.Enums;
using IntegradorApi.Models;
using IntegradorApi.Services;
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
    private ObservableCollection<Connection> SourceConnections { get; set; } = new();


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
      var serverVersion = new MySqlServerVersion(new Version(8, 1, 0));
      optionsBuilder.UseMySql(connectionString, serverVersion);

      _databaseService = new DatabaseService(optionsBuilder.Options);

      var hwnd = WindowNative.GetWindowHandle(this);
      var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
      _appWindow = AppWindow.GetFromWindowId(windowId);

      TrayMenuFlyout.XamlRoot = this.Content.XamlRoot;
      MyTaskbarIcon.DataContext = this;

      LogEventBus.OnLogReceived += AppendLogToUI;
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
        Server = TxtLocalDataBaseAddress.Text,
        Port = Convert.ToUInt32(TxtLocalDataBasePort.Text),
        UserID = TxtLocalDataBaseUser.Text,
        Password = PswLocalDataBasePassword.Password,
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
            TxtLocalDataBaseDescription.Text,
            TxtLocalDataBaseAddress.Text,
            TxtLocalDataBasePort.Text,
            TxtLocalDataBaseUser.Text,
            PswLocalDataBasePassword.Password
        );

        await ApplyMigrationsAsync();
        await LoadSourceDataGridAsync();
        await ShowMessageDialog("Sucesso", "As configurações de conexão foram salvas com sucesso!");
      } catch (Exception ex) {
        await ShowMessageDialog("Erro ao Salvar", $"Ocorreu um erro ao salvar as configurações: {ex.Message}");
      }
    }

    private async void Click_AddSource(object sender, RoutedEventArgs e) {
      if (OrigemTipoComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(OrigemEnderecoTextBox.Text)) {
        await ShowMessageDialog("Dados Incompletos", "Por favor, selecione um tipo e preencha o endereço (URL).");
        return;
      }

      var newConnection = new Connection {
        TypeConnection = (ConnectionType)OrigemTipoComboBox.SelectedIndex,
        Description = OrigemDescricaoTextBox.Text,
        Address = OrigemEnderecoTextBox.Text,
        User = OrigemUserTextBox.Text,
        Password = OrigemPasswordTextBox.Text,
        Optional = OrigemOptionalTextBox.Text,
        Enabled = OrigemEnabledCheckBox.IsChecked ?? false
      };

      await _databaseService.AddConnectionAsync(newConnection);
      SourceConnections.Add(newConnection);

      OrigemTipoComboBox.SelectedIndex = -1;
      OrigemDescricaoTextBox.Text = string.Empty;
      OrigemEnderecoTextBox.Text = string.Empty;
      OrigemUserTextBox.Text = string.Empty;
      OrigemPasswordTextBox.Text = string.Empty;
      OrigemOptionalTextBox.Text = string.Empty;
      OrigemEnabledCheckBox.IsChecked = true;
    }

    private async void Click_DeleteSource(object sender, RoutedEventArgs e) {
      var selectedConnection = OrigemDataGrid.SelectedItem as Connection;
      if (selectedConnection == null) {
        await ShowMessageDialog("Nenhum item selecionado", "Por favor, selecione uma conexão na grid para excluir.");
        return;
      }

      await _databaseService.DeleteConnectionAsync(selectedConnection);
      SourceConnections.Remove(selectedConnection);
    }

    private async void Click_SourceEnabled(object sender, RoutedEventArgs e) {
      var checkBox = sender as CheckBox;
      var connectionToUpdate = checkBox?.DataContext as Connection;
      if (connectionToUpdate != null)
        await _databaseService.UpdateConnectionAsync(connectionToUpdate);
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

    private void AppendLogToUI(string message) {
      DispatcherQueue.TryEnqueue(() => {
        LogTextBlock.Text += message;
      });
    }

    private void ReadConfiguration() {
      TxtLocalDataBaseDescription.Text = AppConfig.Configuration.GetConnectionString("LocalDatabase:Description");
      TxtLocalDataBaseAddress.Text = AppConfig.Configuration.GetConnectionString("LocalDatabase:Address");
      TxtLocalDataBasePort.Text = AppConfig.Configuration.GetConnectionString("LocalDatabase:Port");
      TxtLocalDataBaseUser.Text = AppConfig.Configuration.GetConnectionString("LocalDatabase:User");
      PswLocalDataBasePassword.Password = AppConfig.Configuration.GetConnectionString("LocalDatabase:Password");
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
        await using var context = new AppDbContext();
        await context.Database.MigrateAsync();
        Log.Information("Banco de dados está atualizado.");
      } catch (System.Exception ex) {
        Log.Fatal(ex, "Falha ao aplicar migrações do banco de dados. Verifique as configurações de conexão.");
        await ShowMessageDialog("Erro de Conexão", $"Não foi possível conectar ou atualizar o banco de dados. Por favor, verifique os dados na aba 'Conexões'.\n\nErro: {ex.Message}");
      }
    }

    private async Task LoadSourceDataGridAsync() {
      try {
        var connections = await _databaseService.GetConnectionsAsync();
        DispatcherQueue.TryEnqueue(() => {
          SourceConnections.Clear();
          foreach (var conn in connections)
            SourceConnections.Add(conn);
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

      // Supondo que você tenha uma lista de conexões (ex: List<ConexaoOrigem> conexoes)
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

    private void RunSincronization(object state) {
      // Log para o Serilog (thread-safe)
      Serilog.Log.Information("Iniciando rotina de sincronização...");

      // Atualiza a UI a partir da thread de background de forma segura
      DispatcherQueue.TryEnqueue(() => {
        LogTextBlock.Text += $"{DateTime.Now:G}: Iniciando rotina de sincronização...\n";
      });

      // ===================================================================
      //
      //      BLOCO DE EXECUÇÃO EM BRANCO - LÓGICA DA SINCRONIZAÇÃO
      //
      //      Este é o local onde você irá futuramente adicionar:
      //      1. A chamada ao DatabaseService para buscar as conexões.
      //      2. O loop para testar e conectar em cada fonte de dados.
      //      3. A lógica para buscar novos dados.
      //      4. A lógica para salvar os dados no destino.
      //      5. A atualização do RegistroSincronizacao com a nova data.
      //
      // ===================================================================

      // Simula um tempo de execução
      System.Threading.Thread.Sleep(5000); // Remover em produção

      // Log de conclusão
      Serilog.Log.Information("Rotina de sincronização concluída.");
      DispatcherQueue.TryEnqueue(() => {
        LogTextBlock.Text += $"{DateTime.Now:G}: Rotina de sincronização concluída.\n";
      });
    }
  }
}
