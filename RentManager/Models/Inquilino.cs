using System;
using System.Collections.Generic;
using System.Text;

namespace RentManager.Models
{
    internal class Inquilino
    {
        public int IdInquilino { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaAlta { get; set; } = DateTime.Now;
        public string Observaciones { get; set; } = string.Empty;
    }
}
