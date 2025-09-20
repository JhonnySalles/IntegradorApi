using IntegradorApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;


namespace IntegradorApi {
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window {

        private readonly SettingsService _settingsService;
        private System.Threading.Timer _syncTimer;

        private readonly TimeSpan _initialSyncDelay = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _recurringSyncInterval = TimeSpan.FromHours(3);

        public MainWindow() {
            InitializeComponent();
            SystemBackdrop = new MicaBackdrop();

            _settingsService = new SettingsService();
            ReadConfiguration();
            LoadSyncButtonState();
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

        private void ButtonClick_ToggleSyncronize(object sender, RoutedEventArgs e) {
            bool currentState = _settingsService.GetSincronizacaoStatus();
            _settingsService.SetSincronizacaoStatus(!currentState);
            LoadSyncButtonState();
        }

        private void StartTimer(bool comDelayInicial) {
            _syncTimer?.Dispose();

            TimeSpan dueTime = comDelayInicial ? _initialSyncDelay : TimeSpan.Zero;
            TimeSpan period = _recurringSyncInterval; // Período de 3 horas

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
