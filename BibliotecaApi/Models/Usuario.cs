using Newtonsoft.Json;

namespace BibliotecaApi.Models
{
    public class Usuario
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string password { get; set; }

        public string rol { get; set; }
    }
}
