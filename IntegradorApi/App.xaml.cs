using IntegradorApi.Data;
using IntegradorApi.Services;
using Microsoft.UI.Xaml;
using Serilog;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IntegradorApi {
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application {
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() {
            LogManager.Logger.Information("Aplicação Iniciando...");
            Migrations();
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) {
            _window = new MainWindow();
            _window.Activate();
        }

        private void Migrations() {
            Log.Information("Verificando e aplicando migrações do banco de dados...");
            try {
                using var context = new AppDbContext();
                //context.Database.Migrate(); // Mágica acontece aqui!
                Log.Information("Banco de dados está atualizado.");
            } catch (System.Exception ex) {
                Log.Fatal(ex, "Falha ao aplicar migrações do banco de dados.");
            }
        }
    }
}
