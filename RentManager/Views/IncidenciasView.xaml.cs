using RentManager.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RentManager.Views
{
    // Vista encargada de mostrar y gestionar incidencias
    public partial class IncidenciasView : UserControl
    {
        private readonly IncidenciaRepository _repo = new IncidenciaRepository();
        private List<IncidenciaListado> _todas = new();

        public IncidenciasView()
        {
            InitializeComponent();
            CargarIncidencias();
        }

        // Carga las incidencias desde la base de datos
        private void CargarIncidencias()
        {
            _todas = _repo.ObtenerTodas();
            dgIncidencias.ItemsSource = _todas;
        }

        private void BtnAñadir_Click(object sender, RoutedEventArgs e)
        {
            var form = new IncidenciaForm { Owner = Window.GetWindow(this) };

            if (form.ShowDialog() == true)
            {
                CargarIncidencias();
                AplicarFiltro();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgIncidencias.SelectedItem is not IncidenciaListado incidencia)
            {
                MessageBox.Show("Selecciona una incidencia para editar.");
                return;
            }

            var form = new IncidenciaForm(incidencia) { Owner = Window.GetWindow(this) };

            if (form.ShowDialog() == true)
            {
                CargarIncidencias();
                AplicarFiltro();
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgIncidencias.SelectedItem is not IncidenciaListado incidencia)
            {
                MessageBox.Show("Selecciona una incidencia para eliminar.");
                return;
            }

            var confirm = MessageBox.Show(
                "¿Seguro que quieres eliminar la incidencia seleccionada?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm == MessageBoxResult.Yes)
            {
                _repo.Eliminar(incidencia.IdIncidencia);
                CargarIncidencias();
                AplicarFiltro();
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            AplicarFiltro();
        }

        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltro();
        }

        // Aplica un filtro de búsqueda por vivienda, título, descripción o estado
        private void AplicarFiltro()
        {
            var texto = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                dgIncidencias.ItemsSource = _todas;
                return;
            }

            var filtradas = _todas.FindAll(i =>
                i.ViviendaDireccion.ToLower().Contains(texto) ||
                i.Titulo.ToLower().Contains(texto) ||
                i.Descripcion.ToLower().Contains(texto) ||
                i.Estado.ToString().ToLower().Contains(texto)
            );

            dgIncidencias.ItemsSource = filtradas;
        }
    }
}