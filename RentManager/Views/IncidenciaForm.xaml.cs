using RentManager.Data;
using RentManager.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace RentManager.Views
{
    // Ventana para crear o editar una incidencia
    public partial class IncidenciaForm : Window
    {
        private readonly ViviendaRepository _repoVivienda = new ViviendaRepository();
        private readonly IncidenciaRepository _repoIncidencia = new IncidenciaRepository();

        private readonly IncidenciaListado? _incidenciaEditar;

        public IncidenciaForm()
        {
            InitializeComponent();
            CargarViviendas();

            dpFecha.SelectedDate = DateTime.Today;
            cmbEstado.SelectedIndex = 0;
        }

        public IncidenciaForm(IncidenciaListado incidencia) : this()
        {
            txtTituloVentana.Text = "Editar incidencia";
            _incidenciaEditar = incidencia;

            cmbVivienda.SelectedValue = incidencia.IdVivienda;
            txtTitulo.Text = incidencia.Titulo;
            txtDescripcion.Text = incidencia.Descripcion;
            dpFecha.SelectedDate = incidencia.Fecha;
            txtCoste.Text = incidencia.CosteEstimado.ToString();
            txtObservaciones.Text = incidencia.Observaciones;

            cmbEstado.SelectedIndex = incidencia.Estado switch
            {
                EstadoIncidencia.EnProceso => 1,
                EstadoIncidencia.Resuelta => 2,
                _ => 0
            };
        }

        // Carga las viviendas disponibles en el desplegable
        private void CargarViviendas()
        {
            var viviendas = _repoVivienda.ObtenerTodas();
            cmbVivienda.ItemsSource = viviendas;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar())
                return;

            var incidencia = new Incidencia
            {
                IdVivienda = (int)cmbVivienda.SelectedValue,
                Titulo = txtTitulo.Text.Trim(),
                Descripcion = txtDescripcion.Text.Trim(),
                Fecha = dpFecha.SelectedDate!.Value,
                Estado = ObtenerEstadoSeleccionado(),
                CosteEstimado = string.IsNullOrWhiteSpace(txtCoste.Text) ? 0 : decimal.Parse(txtCoste.Text),
                Observaciones = txtObservaciones.Text.Trim()
            };

            if (_incidenciaEditar == null)
            {
                _repoIncidencia.Insertar(incidencia);
            }
            else
            {
                incidencia.IdIncidencia = _incidenciaEditar.IdIncidencia;
                _repoIncidencia.Actualizar(incidencia);
            }

            DialogResult = true;
            Close();
        }

        // Valida que los datos obligatorios sean correctos
        private bool Validar()
        {
            if (cmbVivienda.SelectedValue == null)
            {
                MessageBox.Show("Selecciona una vivienda.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MessageBox.Show("Introduce un título para la incidencia.");
                return false;
            }

            if (dpFecha.SelectedDate == null)
            {
                MessageBox.Show("Selecciona una fecha.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtCoste.Text) && !decimal.TryParse(txtCoste.Text, out _))
            {
                MessageBox.Show("El coste estimado debe ser un número válido.");
                return false;
            }

            return true;
        }

        // Devuelve el estado seleccionado en el desplegable
        private EstadoIncidencia ObtenerEstadoSeleccionado()
        {
            var estado = ((ComboBoxItem)cmbEstado.SelectedItem).Content.ToString();

            return estado switch
            {
                "EnProceso" => EstadoIncidencia.EnProceso,
                "Resuelta" => EstadoIncidencia.Resuelta,
                _ => EstadoIncidencia.Abierta
            };
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}