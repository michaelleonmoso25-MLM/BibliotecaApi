using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Models
{
    public class Libro
    {
        public int id { get; set; }

        [Required]
        [StringLength(200)]
        public string titulo { get; set; }

        [StringLength(150)]
        public string autor { get; set; }

        [StringLength(20)]
        public string isbn { get; set; }

        [StringLength(80)]
        public string categoria { get; set; }

        [StringLength(300)]
        public string portadaUrl { get; set; }

        public bool disponible { get; set; }
    }
}
