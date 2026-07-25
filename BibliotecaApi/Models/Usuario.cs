using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BibliotecaApi.Models
{
    public class Usuario
    {
        public int id { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string correo { get; set; }

        // Solo entra (registro / cambio de contraseña); nunca se serializa hacia el cliente.
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string password { get; set; }

        [Required]
        [RegularExpression("Bibliotecario|Lector", ErrorMessage = "El rol debe ser 'Bibliotecario' o 'Lector'.")]
        public string rol { get; set; }
    }
}
