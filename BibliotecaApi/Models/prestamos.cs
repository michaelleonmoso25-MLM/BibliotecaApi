using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BibliotecaApi.Models
{
    public class Prestamo
    {
        public int id { get; set; }
        public int libroId { get; set; }
        public int usuarioId { get; set; }
        public string fechaPrestamo { get; set; }
        public string fechaLimite { get; set; }
        public string fechaDevolucion { get; set; }
        public string estado { get; set; }
    }
}