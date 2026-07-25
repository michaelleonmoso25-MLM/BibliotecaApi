using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Models
{
    public class LoginRequest
    {
        [Required]
        public string correo { get; set; }

        [Required]
        public string password { get; set; }
    }
}
