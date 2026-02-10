using System;

namespace RentManager.Models
{
    // Representa una vivienda de la aplicación
    public class Vivienda
    {
        public int IdVivienda { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public decimal PrecioMensual { get; set; }
        public string Estado { get; set; } = "Libre";
        public string Observaciones { get; set; } = string.Empty;
        public DateTime FechaAlta { get; set; } = DateTime.Now;
    }
}
