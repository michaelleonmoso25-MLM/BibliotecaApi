using BibliotecaApi.Data;
using BibliotecaApi.Filters;
using BibliotecaApi.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BibliotecaApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Autorizar] // requiere usuario autenticado en todos los endpoints
    public class LibroController : ApiController
    {
        LibroDAO dao = new LibroDAO();

        // Obtener todo -> cualquier usuario autenticado (Lector o Bibliotecario)
        public IEnumerable<Libro> Get()
        {
            return dao.GetAll();
        }

        // Obtener por Id
        public IHttpActionResult Get(int id)
        {
            var libro = dao.GetById(id);
            if (libro == null) return NotFound();
            return Ok(libro);
        }

        // Insertar -> solo Bibliotecario
        [Autorizar(Roles = "Bibliotecario")]
        public IHttpActionResult Post(Libro libro)
        {
            if (libro == null || string.IsNullOrWhiteSpace(libro.titulo))
                return BadRequest("El título es obligatorio.");
            dao.Insert(libro);
            return StatusCode(System.Net.HttpStatusCode.Created);
        }

        // Actualizar -> solo Bibliotecario
        [Autorizar(Roles = "Bibliotecario")]
        public IHttpActionResult Put(int id, Libro libro)
        {
            if (dao.GetById(id) == null) return NotFound();
            dao.Update(id, libro);
            return Ok(dao.GetById(id));
        }

        // Borrar -> solo Bibliotecario
        [Autorizar(Roles = "Bibliotecario")]
        public IHttpActionResult Delete(int id)
        {
            if (dao.GetById(id) == null) return NotFound();
            dao.Delete(id);
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }
    }
}