using System;

namespace BibliotecaApi.Models
{
    // DTO de solo lectura para mostrar préstamos con los datos relacionados
    // del libro y del usuario (resultado de un JOIN).
    public class PrestamoDetalle
    {
        public int id { get; set; }

        public int libroId { get; set; }
        public string libroTitulo { get; set; }
        public string libroAutor { get; set; }

        public int usuarioId { get; set; }
        public string usuarioNombre { get; set; }
        public string usuarioCorreo { get; set; }

        public DateTime fechaPrestamo { get; set; }
        public DateTime fechaLimite { get; set; }
        public DateTime? fechaDevolucion { get; set; }
        public string estado { get; set; }

        // Calculado: true si sigue activo y ya pasó la fecha límite.
        public bool vencido { get; set; }
    }
}
