using RentManager.Data;
using RentManager.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace RentManager.Views
{
    // Ventana para crear o editar una vivienda
    public partial class ViviendaForm : Window
    {
        private readonly ViviendaRepository _repo = new ViviendaRepository();
        private readonly Vivienda? _viviendaEditar;

        // Constructor para nueva vivienda
        public ViviendaForm()
        {
            InitializeComponent();
            cmbEstado.SelectedIndex = 0;
        }

        // Constructor para editar vivienda existente
        public ViviendaForm(Vivienda vivienda) : this()
        {
            txtTitulo.Text = "Editar vivienda";
            _viviendaEditar = vivienda;

            txtDireccion.Text = vivienda.Direccion;
            txtCiudad.Text = vivienda.Ciudad;
            txtCodigoPostal.Text = vivienda.CodigoPostal;
            txtPrecio.Text = vivienda.PrecioMensual.ToString(CultureInfo.InvariantCulture);
            cmbEstado.SelectedIndex = vivienda.Estado == "Alquilada" ? 1 : 0;
            txtObservaciones.Text = vivienda.Observaciones;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar())
                return;

            var vivienda = new Vivienda
            {
                Direccion = txtDireccion.Text.Trim(),
                Ciudad = txtCiudad.Text.Trim(),
                CodigoPostal = txtCodigoPostal.Text.Trim(),
                PrecioMensual = decimal.Parse(txtPrecio.Text, CultureInfo.InvariantCulture),
                Estado = ((ComboBoxItem)cmbEstado.SelectedItem).Content.ToString()!,
                Observaciones = txtObservaciones.Text.Trim(),
                FechaAlta = DateTime.Now
            };

            if (_viviendaEditar == null)
            {
                _repo.Insertar(vivienda);
            }
            else
            {
                vivienda.IdVivienda = _viviendaEditar.IdVivienda;
                _repo.Actualizar(vivienda);
            }

            DialogResult = true;
            Close();
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                string.IsNullOrWhiteSpace(txtCiudad.Text) ||
                string.IsNullOrWhiteSpace(txtCodigoPostal.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Rellena todos los campos obligatorios.");
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text, out _))
            {
                MessageBox.Show("El precio debe ser un número válido.");
                return false;
            }

            return true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
