using System;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Models
{
    public class Prestamo
    {
        public int id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "libroId es obligatorio.")]
        public int libroId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "usuarioId es obligatorio.")]
        public int usuarioId { get; set; }

        public DateTime fechaPrestamo { get; set; }
        public DateTime fechaLimite { get; set; }
        public DateTime? fechaDevolucion { get; set; }
        public string estado { get; set; }
    }
}
