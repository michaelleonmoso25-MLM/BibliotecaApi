using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using BibliotecaApi.Data;
using BibliotecaApi.Filters;
using BibliotecaApi.Models;
using BibliotecaApi.Security;

namespace BibliotecaApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsuarioController : ApiController
    {
        UsuarioDAO dao = new UsuarioDAO();

        // POST api/usuario/login  -> público
        [HttpPost]
        [Route("api/usuario/login")]
        public IHttpActionResult Login(LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.correo) || string.IsNullOrWhiteSpace(req.password))
                return BadRequest("Correo y contraseña son obligatorios.");

            var usuario = dao.GetByCorreoConPassword(req.correo);
            if (usuario == null || !PasswordHelper.Verify(req.password, usuario.password))
                return Unauthorized();

            string token = TokenHelper.Generate(usuario);
            return Ok(new
            {
                token,
                usuario = new { usuario.id, usuario.nombre, usuario.correo, usuario.rol }
            });
        }

        // GET api/usuario  -> solo Bibliotecario
        [Autorizar(Roles = "Bibliotecario")]
        public IEnumerable<Usuario> Get()
        {
            return dao.GetAll();
        }

        // GET api/usuario/{id} -> solo Bibliotecario
        [Autorizar(Roles = "Bibliotecario")]
        public IHttpActionResult Get(int id)
        {
            var u = dao.GetById(id);
            if (u == null) return NotFound();
            return Ok(u);
        }

        // POST api/usuario -> solo Bibliotecario (registro de usuarios)
        [Autorizar(Roles = "Bibliotecario")]
        public IHttpActionResult Post(Usuario usuario)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.correo)
                || string.IsNullOrWhiteSpace(usuario.password) || string.IsNullOrWhiteSpace(usuario.nombre)
                || string.IsNullOrWhiteSpace(usuario.rol))
                return BadRequest("nombre, correo, password y rol son obligatorios.");

            if (dao.CorreoExiste(usuario.correo))
                return Conflict();

            int id = dao.Insert(usuario);
            var creado = dao.GetById(id);
            return Created(Request.RequestUri + "/" + id, creado);
        }

        // PUT api/usuario/{id} -> solo Bibliotecario
        [Autorizar(Roles = "Bibliotecario")]
        public IHttpActionResult Put(int id, Usuario usuario)
        {
            if (usuario == null) return BadRequest("Datos inválidos.");
            if (dao.GetById(id) == null) return NotFound();

            dao.Update(id, usuario);
            return Ok(dao.GetById(id));
        }

        // DELETE api/usuario/{id} -> solo Bibliotecario
        [Autorizar(Roles = "Bibliotecario")]
        public IHttpActionResult Delete(int id)
        {
            if (dao.GetById(id) == null) return NotFound();
            dao.Delete(id);
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }
    }
}
