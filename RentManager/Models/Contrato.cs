using System;

namespace RentManager.Models
{
    // Representa un contrato de alquiler asociado a una vivienda y un inquilino
    public class Contrato
    {
        public int IdContrato { get; set; }

        public int IdVivienda { get; set; }
        public int IdInquilino { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public decimal RentaMensual { get; set; }
        public decimal? Fianza { get; set; }

        public EstadoContrato Estado { get; set; } = EstadoContrato.Activo;
        public string Observaciones { get; set; } = string.Empty;
    }
}
