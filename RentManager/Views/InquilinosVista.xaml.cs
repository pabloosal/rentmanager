using RentManager.Data;
using RentManager.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RentManager.Views
{
    // Vista encargada de mostrar y gestionar el listado de inquilinos
    public partial class InquilinosView : UserControl
    {
        private readonly InquilinoRepository _repo = new InquilinoRepository();
        private List<Inquilino> _todos = new();

        public InquilinosView()
        {
            InitializeComponent();
            CargarInquilinos();
        }

        // Carga los inquilinos desde la base de datos en el DataGrid
        private void CargarInquilinos()
        {
            _todos = _repo.ObtenerTodos();
            dgInquilinos.ItemsSource = _todos;
        }

        // Ejecuta la búsqueda aplicando el filtro al listado mostrado
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            AplicarFiltro();
        }

        // Abre el formulario para añadir un inquilino y recarga la lista si se guarda
        private void BtnAñadir_Click(object sender, RoutedEventArgs e)
        {
            var form = new InquilinoForm { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true)
            {
                CargarInquilinos();
                AplicarFiltro();
            }
        }

        // Abre el formulario para editar el inquilino seleccionado y recarga la lista si se guarda
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgInquilinos.SelectedItem is not Inquilino inquilino)
            {
                MessageBox.Show("Selecciona un inquilino para editar.");
                return;
            }

            var form = new InquilinoForm(inquilino) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true)
            {
                CargarInquilinos();
                AplicarFiltro();
            }
        }

        // Elimina el inquilino seleccionado tras confirmación y actualiza el listado
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgInquilinos.SelectedItem is not Inquilino inquilino)
            {
                MessageBox.Show("Selecciona un inquilino para eliminar.");
                return;
            }

            var confirm = MessageBox.Show(
                "¿Seguro que quieres eliminar el inquilino seleccionado?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm == MessageBoxResult.Yes)
            {
                _repo.Eliminar(inquilino.IdInquilino);
                CargarInquilinos();
                AplicarFiltro();
            }
        }

        // Aplica el filtro de búsqueda al DataGrid en base al texto introducido
        private void AplicarFiltro()
        {
            var texto = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                dgInquilinos.ItemsSource = _todos;
                return;
            }

            var filtrados = _todos.FindAll(i =>
                i.Nombre.ToLower().Contains(texto) ||
                i.Apellidos.ToLower().Contains(texto) ||
                i.Dni.ToLower().Contains(texto) ||
                i.Telefono.ToLower().Contains(texto) ||
                i.Email.ToLower().Contains(texto)
            );

            dgInquilinos.ItemsSource = filtrados;
        }
    }
}
