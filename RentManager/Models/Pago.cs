using System;

namespace RentManager.Models
{
    // Representa un pago mensual asociado a un contrato
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdContrato { get; set; }

        public int Mes { get; set; }
        public int Anyo { get; set; }

        public decimal Importe { get; set; }
        public EstadoPago Estado { get; set; } = EstadoPago.Pendiente;

        public DateTime? FechaPago { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }
}
