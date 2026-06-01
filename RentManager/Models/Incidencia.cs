using System;

namespace RentManager.Models
{
    // Representa una incidencia asociada a una vivienda
    public class Incidencia
    {
        public int IdIncidencia { get; set; }
        public int IdVivienda { get; set; }

        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;
        public EstadoIncidencia Estado { get; set; } = EstadoIncidencia.Abierta;

        public decimal CosteEstimado { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }
}