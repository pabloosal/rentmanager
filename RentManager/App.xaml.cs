using System.Windows;
using RentManager.Data;
using RentManager.Views;

namespace RentManager
{
    // Clase principal de la aplicación
    public partial class App : Application
    {
        // Inicializa la aplicación y muestra la ventana de inicio de sesión
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Db.Initialize();
            new Login().Show();
        }
    }
}