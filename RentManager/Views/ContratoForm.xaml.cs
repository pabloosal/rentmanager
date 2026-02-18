using RentManager.Data;
using RentManager.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RentManager.Views
{
    // Ventana para crear o editar un contrato
    public partial class ContratoForm : Window
    {
        private readonly ContratoRepository _repoContrato = new ContratoRepository();
        private readonly ViviendaRepository _repoVivienda = new ViviendaRepository();
        private readonly InquilinoRepository _repoInquilino = new InquilinoRepository();

        private readonly Contrato? _contratoEditar;

        public ContratoForm()
        {
            InitializeComponent();
            CargarCombos();

            dpInicio.SelectedDate = DateTime.Today;
            cmbEstado.SelectedIndex = 0;
        }

        public ContratoForm(ContratoListado contratoListado) : this()
        {
            txtTitulo.Text = "Editar contrato";

            // Convertimos el listado a un contrato base para editar
            _contratoEditar = new Contrato
            {
                IdContrato = contratoListado.IdContrato,
                IdVivienda = contratoListado.IdVivienda,
                IdInquilino = contratoListado.IdInquilino,
                FechaInicio = contratoListado.FechaInicio,
                FechaFin = contratoListado.FechaFin,
                RentaMensual = contratoListado.RentaMensual,
                Estado = contratoListado.Estado
            };

            // Seleccionar valores en los combos
            SeleccionarEnComboPorId(cmbVivienda, _contratoEditar.IdVivienda);
            SeleccionarEnComboPorId(cmbInquilino, _contratoEditar.IdInquilino);

            dpInicio.SelectedDate = _contratoEditar.FechaInicio;
            dpFin.SelectedDate = _contratoEditar.FechaFin;

            txtRenta.Text = _contratoEditar.RentaMensual.ToString(CultureInfo.CurrentCulture);
            txtFianza.Text = _contratoEditar.Fianza?.ToString(CultureInfo.CurrentCulture) ?? "";

            cmbEstado.SelectedIndex = _contratoEditar.Estado == EstadoContrato.Finalizado ? 1 : 0;
        }

        private void CargarCombos()
        {
            // Viviendas: mostramos dirección pero guardamos el ID
            var viviendas = _repoVivienda.ObtenerTodas();
            cmbVivienda.ItemsSource = viviendas;
            cmbVivienda.DisplayMemberPath = "Direccion";
            cmbVivienda.SelectedValuePath = "IdVivienda";

            // Inquilinos: mostramos nombre completo
            var inquilinos = _repoInquilino.ObtenerTodos();
            cmbInquilino.ItemsSource = inquilinos;
            cmbInquilino.DisplayMemberPath = "Nombre"; // mejoraremos en la vista si quieres
            cmbInquilino.SelectedValuePath = "IdInquilino";
        }

        private static void SeleccionarEnComboPorId(ComboBox combo, int id)
        {
            combo.SelectedValue = id;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar())
                return;

            var contrato = new Contrato
            {
                IdVivienda = (int)cmbVivienda.SelectedValue,
                IdInquilino = (int)cmbInquilino.SelectedValue,
                FechaInicio = dpInicio.SelectedDate!.Value,
                FechaFin = dpFin.SelectedDate,
                RentaMensual = decimal.Parse(txtRenta.Text),
                Fianza = string.IsNullOrWhiteSpace(txtFianza.Text) ? null : decimal.Parse(txtFianza.Text),
                Estado = ((ComboBoxItem)cmbEstado.SelectedItem).Content.ToString() == "Finalizado"
                    ? EstadoContrato.Finalizado
                    : EstadoContrato.Activo,
                Observaciones = txtObservaciones.Text.Trim()
            };

            if (_contratoEditar == null)
            {
                _repoContrato.Insertar(contrato);
            }
            else
            {
                contrato.IdContrato = _contratoEditar.IdContrato;
                _repoContrato.Actualizar(contrato);
            }

            DialogResult = true;
            Close();
        }

        private bool Validar()
        {
            if (cmbVivienda.SelectedValue == null || cmbInquilino.SelectedValue == null)
            {
                MessageBox.Show("Selecciona una vivienda y un inquilino.");
                return false;
            }

            if (dpInicio.SelectedDate == null)
            {
                MessageBox.Show("Selecciona una fecha de inicio.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRenta.Text) || !decimal.TryParse(txtRenta.Text, out _))
            {
                MessageBox.Show("La renta mensual debe ser un número válido.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtFianza.Text) && !decimal.TryParse(txtFianza.Text, out _))
            {
                MessageBox.Show("La fianza debe ser un número válido.");
                return false;
            }

            if (dpFin.SelectedDate != null && dpFin.SelectedDate < dpInicio.SelectedDate)
            {
                MessageBox.Show("La fecha fin no puede ser anterior a la fecha inicio.");
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
