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
                stackPanel.Children.Add(new TextBlock { Text = "Pausar Sincroniza��es" });
                BtnToggleSync.Content = stackPanel;
                StartTimer(comDelayInicial: true);
            } else {
                stackPanel.Children.Add(new SymbolIcon(Symbol.Play));
                stackPanel.Children.Add(new TextBlock { Text = "Iniciar Sincroniza��es" });
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
            TimeSpan period = _recurringSyncInterval; // Per�odo de 3 horas

            _syncTimer = new System.Threading.Timer(RunSincronization, null, dueTime, period);

            Serilog.Log.Information("Timer de sincroniza��o iniciado. Pr�xima execu��o em {DueTime}.", dueTime);
            DispatcherQueue.TryEnqueue(() => {
                LogTextBlock.Text += $"{DateTime.Now:G}: Timer de sincroniza��o iniciado. Pr�xima execu��o em {dueTime}.\n";
            });
        }

        private void StopTimer() {
            _syncTimer?.Dispose();
            _syncTimer = null; // Anula a refer�ncia

            Serilog.Log.Information("Timer de sincroniza��o parado.");
            DispatcherQueue.TryEnqueue(() => {
                LogTextBlock.Text += $"{DateTime.Now:G}: Timer de sincroniza��o parado.\n";
            });
        }

        private void SincronizeManual() {
            Serilog.Log.Information("Sincroniza��o manual acionada.");
            Task.Run(() => RunSincronization(null));

            if (_settingsService.GetSincronizacaoStatus()) {
                _syncTimer?.Change(_recurringSyncInterval, _recurringSyncInterval);
                Serilog.Log.Information("Timer resetado. Pr�xima execu��o autom�tica em {Intervalo}", _recurringSyncInterval);
            }
        }


        // A l�gica para os �cones de status seria implementada nos m�todos
        // que s�o chamados pelos bot�es "Testar Conex�es" ou pelo pr�prio processo de sincroniza��o.
        // Exemplo:
        private void UpdateConnectionStatusIcons() {
            // Limpa os �cones atuais
            StatusConectionsItemsControl.Items.Clear();

            // Supondo que voc� tenha uma lista de conex�es (ex: List<ConexaoOrigem> conexoes)
            // foreach (var conexao in conexoes)
            // {
            //     var icon = new FontIcon
            //     {
            //         FontFamily = new FontFamily("Segoe MDL2 Assets"),
            //         Glyph = "\uE753", // �cone de "Plug"
            //         FontSize = 24
            //     };
            //
            //     // Define a cor baseada no status (l�gica a ser implementada)
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
            Serilog.Log.Information("Iniciando rotina de sincroniza��o...");

            // Atualiza a UI a partir da thread de background de forma segura
            DispatcherQueue.TryEnqueue(() => {
                LogTextBlock.Text += $"{DateTime.Now:G}: Iniciando rotina de sincroniza��o...\n";
            });

            // ===================================================================
            //
            //      BLOCO DE EXECU��O EM BRANCO - L�GICA DA SINCRONIZA��O
            //
            //      Este � o local onde voc� ir� futuramente adicionar:
            //      1. A chamada ao DatabaseService para buscar as conex�es.
            //      2. O loop para testar e conectar em cada fonte de dados.
            //      3. A l�gica para buscar novos dados.
            //      4. A l�gica para salvar os dados no destino.
            //      5. A atualiza��o do RegistroSincronizacao com a nova data.
            //
            // ===================================================================

            // Simula um tempo de execu��o
            System.Threading.Thread.Sleep(5000); // Remover em produ��o

            // Log de conclus�o
            Serilog.Log.Information("Rotina de sincroniza��o conclu�da.");
            DispatcherQueue.TryEnqueue(() => {
                LogTextBlock.Text += $"{DateTime.Now:G}: Rotina de sincroniza��o conclu�da.\n";
            });
        }
    }
}
