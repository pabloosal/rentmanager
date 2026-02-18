using RentManager.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RentManager.Views
{
    // Vista encargada de mostrar y gestionar el listado de contratos
    public partial class ContratosView : UserControl
    {
        private readonly ContratoRepository _repo = new ContratoRepository();
        private List<ContratoListado> _todos = new();

        public ContratosView()
        {
            InitializeComponent();
            CargarContratos();
        }

        private void CargarContratos()
        {
            _todos = _repo.ObtenerTodos();
            dgContratos.ItemsSource = _todos;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            AplicarFiltro();
        }

        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            var texto = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                dgContratos.ItemsSource = _todos;
                return;
            }

            var filtrados = _todos.FindAll(c =>
                c.ViviendaDireccion.ToLower().Contains(texto) ||
                c.InquilinoNombreCompleto.ToLower().Contains(texto) ||
                c.Estado.ToString().ToLower().Contains(texto)
            );

            dgContratos.ItemsSource = filtrados;
        }

        private void BtnAñadir_Click(object sender, RoutedEventArgs e)
        {
            var form = new ContratoForm { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true)
            {
                CargarContratos();
                AplicarFiltro();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgContratos.SelectedItem is not ContratoListado contrato)
            {
                MessageBox.Show("Selecciona un contrato para editar.");
                return;
            }

            var form = new ContratoForm(contrato) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true)
            {
                CargarContratos();
                AplicarFiltro();
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgContratos.SelectedItem is not ContratoListado contrato)
            {
                MessageBox.Show("Selecciona un contrato para eliminar.");
                return;
            }

            var confirm = MessageBox.Show(
                "¿Seguro que quieres eliminar el contrato seleccionado?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm == MessageBoxResult.Yes)
            {
                _repo.Eliminar(contrato.IdContrato);
                CargarContratos();
                AplicarFiltro();
            }
        }
    }
}
