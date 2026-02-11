using RentManager.Data;
using RentManager.Models;
using System;
using System.Windows;

namespace RentManager.Views
{
    // Ventana que permite crear o editar un inquilino
    public partial class InquilinoForm : Window
    {
        private readonly InquilinoRepository _repo = new InquilinoRepository();
        private readonly Inquilino? _inquilino;

        // Constructor para crear un inquilino nuevo
        public InquilinoForm()
        {
            InitializeComponent();
            txtTitulo.Text = "Nuevo inquilino";
        }

        // Constructor para editar un inquilino existente
        public InquilinoForm(Inquilino inquilino)
        {
            InitializeComponent();
            _inquilino = inquilino;
            txtTitulo.Text = "Editar inquilino";
            CargarInquilino();
        }

        // Rellena el formulario con los datos del inquilino seleccionado
        private void CargarInquilino()
        {
            if (_inquilino is null) return;

            txtNombre.Text = _inquilino.Nombre;
            txtApellidos.Text = _inquilino.Apellidos;
            txtDni.Text = _inquilino.Dni;
            txtTelefono.Text = _inquilino.Telefono;
            txtEmail.Text = _inquilino.Email;
            txtObservaciones.Text = _inquilino.Observaciones;
        }

        // Cierra la ventana sin guardar
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Guarda el inquilino (insertar o actualizar) validando los campos obligatorios
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            var nombre = txtNombre.Text.Trim();
            var apellidos = txtApellidos.Text.Trim();
            var dni = txtDni.Text.Trim();
            var telefono = txtTelefono.Text.Trim();
            var email = txtEmail.Text.Trim();
            var observaciones = txtObservaciones.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellidos) ||
                string.IsNullOrWhiteSpace(dni))
            {
                MessageBox.Show("Nombre, apellidos y DNI son obligatorios.");
                return;
            }

            try
            {
                if (_inquilino is null)
                {
                    var nuevo = new Inquilino
                    {
                        Nombre = nombre,
                        Apellidos = apellidos,
                        Dni = dni,
                        Telefono = telefono,
                        Email = email,
                        Observaciones = observaciones,
                        FechaAlta = DateTime.Now
                    };

                    _repo.Insertar(nuevo);
                }
                else
                {
                    _inquilino.Nombre = nombre;
                    _inquilino.Apellidos = apellidos;
                    _inquilino.Dni = dni;
                    _inquilino.Telefono = telefono;
                    _inquilino.Email = email;
                    _inquilino.Observaciones = observaciones;

                    _repo.Actualizar(_inquilino);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el inquilino: {ex.Message}");
            }
        }
    }
}
