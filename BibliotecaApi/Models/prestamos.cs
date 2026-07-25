using System;

namespace BibliotecaApi.Models
{
    public class Prestamo
    {
        public int id { get; set; }
        public int libroId { get; set; }
        public int usuarioId { get; set; }
        public DateTime fechaPrestamo { get; set; }
        public DateTime fechaLimite { get; set; }
        public DateTime? fechaDevolucion { get; set; }
        public string estado { get; set; }
    }
}
