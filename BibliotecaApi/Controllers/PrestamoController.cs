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
    public class PrestamoController : ApiController
    {
        PrestamoDAO dao = new PrestamoDAO();

        // Obtener todo
        public IEnumerable<Prestamo> Get()
        {
            return dao.GetAll();
        }

        // Obtener por Id
        public Prestamo Get(int id)
        {
            return dao.GetById(id);
        }

        // Insertar
        public void Post(Prestamo prestamo)
        {
            dao.Insert(prestamo);
        }

        // Actualizar
        public void Put(int id, Prestamo prestamo)
        {
            dao.Update(id, prestamo);
        }

        // Borrar
        public void Delete(int id)
        {
            dao.Delete(id);
        }
    }
}