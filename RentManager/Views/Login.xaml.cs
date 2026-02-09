using System.Windows;
using RentManager.Data;

namespace RentManager.Views
{
    // Ventana de inicio de sesión de la aplicación
    public partial class Login : Window
    {
        // Repositorio para validar el usuario
        private readonly UsuarioRepository _usuarioRepository = new UsuarioRepository();

        public Login()
        {
            InitializeComponent();
        }

        // Se ejecuta al pulsar el botón de iniciar sesión
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var email = txtEmail.Text.Trim();
            var password = txtPassword.Password;

            // Comprueba que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Introduce el email y la contraseña.");
                return;
            }

            // Valida las credenciales en la base de datos
            var ok = _usuarioRepository.ValidarLogin(email, password);

            if (ok)
            {
                new MainWindow().Show();
                Close();
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas.");
            }
        }
    }
}
