using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BibliotecaApi.Models
{
    public class Libro
    {
        public int id { get; set; }
        public string titulo { get; set; }
        public string autor { get; set; }
        public string isbn { get; set; }
        public string categoria { get; set; }
        public string portadaUrl { get; set; }
        public bool disponible { get; set; }
    }
}