using BibliotecaApi.Data;
using BibliotecaApi.Filters;
using BibliotecaApi.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BibliotecaApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Autorizar] // todos los endpoints requieren usuario autenticado
    public class PrestamoController : ApiController
    {
        PrestamoDAO dao = new PrestamoDAO();

        // GET api/prestamo -> cualquier usuario autenticado
        public IEnumerable<Prestamo> Get()
        {
            return dao.GetAll();
        }

        // GET api/prestamo/{id}
        public IHttpActionResult Get(int id)
        {
            var p = dao.GetById(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        // POST api/prestamo -> registrar préstamo (solo Bibliotecario)
        [Autorizar(Roles = "Bibliotecario")]
        public IHttpActionResult Post(Prestamo prestamo)
        {
            if (prestamo == null || prestamo.libroId <= 0 || prestamo.usuarioId <= 0)
                return BadRequest("libroId y usuarioId son obligatorios.");

            try
            {
                int id = dao.Prestar(prestamo);
                return Created(Request.RequestUri + "/" + id, dao.GetById(id));
            }
            catch (ReglaNegocioException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/prestamo/{id}/devolver -> registrar devolución (solo Bibliotecario)
        [HttpPut]
        [Route("api/prestamo/{id}/devolver")]
        [Autorizar(Roles = "Bibliotecario")]
        public IHttpActionResult Devolver(int id)
        {
            try
            {
                dao.Devolver(id);
                return Ok(dao.GetById(id));
            }
            catch (ReglaNegocioException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/prestamo/{id} -> solo Bibliotecario
        [Autorizar(Roles = "Bibliotecario")]
        public IHttpActionResult Delete(int id)
        {
            bool eliminado = dao.Delete(id);
            if (!eliminado) return NotFound();
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }
    }
}
