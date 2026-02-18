using RentManager.Data;
using RentManager.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RentManager.Views
{
    // Vista encargada de mostrar y gestionar los pagos
    public partial class PagosView : UserControl
    {
        private readonly ContratoRepository _repoContrato = new ContratoRepository();
        private readonly PagoRepository _repoPago = new PagoRepository();

        private List<PagoListado> _todos = new();

        private bool _cargando = true;

        public PagosView()
        {
            InitializeComponent();
            _cargando = true;

            CargarContratos();
            txtAnyo.Text = DateTime.Now.Year.ToString();

            _cargando = false;
            CargarPagos();
        }

        private void CargarContratos()
        {
            var contratos = _repoContrato.ObtenerParaCombo();
            cmbContrato.ItemsSource = contratos;
            if (contratos.Count > 0)
                cmbContrato.SelectedIndex = 0;
        }

        private void CargarPagos()
        {
            int? idContrato = cmbContrato.SelectedValue as int?;
            int? anyo = int.TryParse(txtAnyo.Text, out var a) ? a : (int?)null;

            EstadoPago? estado = null;
            var estadoTexto = ((ComboBoxItem)cmbEstado.SelectedItem).Content.ToString();
            if (estadoTexto == "Pendiente") estado = EstadoPago.Pendiente;
            if (estadoTexto == "Pagado") estado = EstadoPago.Pagado;

            _todos = _repoPago.ObtenerTodos(idContrato, anyo, estado);
            dgPagos.ItemsSource = _todos;
        }

        private void Filtros_Changed(object sender, EventArgs e)
        {
            if (_cargando) return;
            CargarPagos();
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbContrato.SelectedValue is not int idContrato)
            {
                MessageBox.Show("Selecciona un contrato.");
                return;
            }

            if (!int.TryParse(txtAnyo.Text, out var anyo))
            {
                MessageBox.Show("Introduce un año válido.");
                return;
            }

            _repoPago.GenerarPagosMensuales(idContrato, anyo);
            CargarPagos();
        }

        private void BtnMarcarPagado_Click(object sender, RoutedEventArgs e)
        {
            if (dgPagos.SelectedItem is not PagoListado pago)
            {
                MessageBox.Show("Selecciona un pago.");
                return;
            }

            if (pago.Estado == EstadoPago.Pagado)
            {
                MessageBox.Show("Ese pago ya está marcado como pagado.");
                return;
            }

            _repoPago.MarcarComoPagado(pago.IdPago, DateTime.Now);
            CargarPagos();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPagos.SelectedItem is not PagoListado pago)
            {
                MessageBox.Show("Selecciona un pago para eliminar.");
                return;
            }

            var confirm = MessageBox.Show(
                "¿Seguro que quieres eliminar el pago seleccionado?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm == MessageBoxResult.Yes)
            {
                _repoPago.Eliminar(pago.IdPago);
                CargarPagos();
            }
        }
    }
}
