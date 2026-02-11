using System.Windows;
using RentManager.Views;

namespace RentManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnViviendas_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ViviendasView();
        }

        private void BtnInquilinos_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new InquilinosView();
        }

        private void BtnContratos_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aquí irá Gestión de Contratos");
        }

        private void BtnPagos_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aquí irá Gestión de Pagos");
        }

        private void BtnGastos_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aquí irá Gestión de Gastos");
        }

        private void BtnIncidencias_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aquí irá Gestión de Incidencias");
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Cierra la ventana principal y vuelve al login
            new Login().Show();
            Close();
        }
    }
}
