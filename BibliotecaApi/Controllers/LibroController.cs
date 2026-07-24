using BibliotecaApi.Data;
using BibliotecaApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BibliotecaApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LibroController : ApiController
    {
        LibroDAO dao = new LibroDAO();

        // Obtener todo
        public IEnumerable<Libro> Get()
        {
            return dao.GetAll();
        }

        // Obtener por Id
        public Libro Get(int id)
        {
            return dao.GetById(id);
        }

        // Insertar
        public void Post(Libro libro)
        {
            dao.Insert(libro);
        }

        // Actualizar
        public void Put(int id, Libro libro)
        {
            dao.Update(id, libro);
        }

        // Borrar
        public void Delete(int id)
        {
            dao.Delete(id);
        }
    }
}