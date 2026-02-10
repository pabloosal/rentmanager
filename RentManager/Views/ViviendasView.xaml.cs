using RentManager.Data;
using RentManager.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RentManager.Views
{
    // Vista encargada de mostrar y gestionar el listado de viviendas
    public partial class ViviendasView : UserControl
    {
        private readonly ViviendaRepository _repo = new ViviendaRepository();
        private List<Vivienda> _todas = new();

        public ViviendasView()
        {
            InitializeComponent();
            CargarViviendas();
        }

        // Carga las viviendas desde la base de datos en el DataGrid
        private void CargarViviendas()
        {
            _todas = _repo.ObtenerTodas();
            dgViviendas.ItemsSource = _todas;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            AplicarFiltro();
        }

        private void BtnAñadir_Click(object sender, RoutedEventArgs e)
        {
            var form = new ViviendaForm { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true)
                CargarViviendas();
                AplicarFiltro();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgViviendas.SelectedItem is not Vivienda vivienda)
            {
                MessageBox.Show("Selecciona una vivienda para editar.");
                return;
            }

            var form = new ViviendaForm(vivienda) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true)
                CargarViviendas();
                AplicarFiltro();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgViviendas.SelectedItem is not Vivienda vivienda)
            {
                MessageBox.Show("Selecciona una vivienda para eliminar.");
                return;
            }

            var confirm = MessageBox.Show(
                "¿Seguro que quieres eliminar la vivienda seleccionada?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm == MessageBoxResult.Yes)
            {
                _repo.Eliminar(vivienda.IdVivienda);
                CargarViviendas();
                AplicarFiltro();
            }
        }

        // Método que aplica el filtro de búsqueda al DataGrid
        private void AplicarFiltro()
        {
            var texto = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                dgViviendas.ItemsSource = _todas;
                return;
            }

            var filtradas = _todas.FindAll(v =>
                v.Direccion.ToLower().Contains(texto) ||
                v.Ciudad.ToLower().Contains(texto) ||
                v.CodigoPostal.ToLower().Contains(texto) ||
                v.Estado.ToLower().Contains(texto)
            );

            dgViviendas.ItemsSource = filtradas;
        }
    }
}
