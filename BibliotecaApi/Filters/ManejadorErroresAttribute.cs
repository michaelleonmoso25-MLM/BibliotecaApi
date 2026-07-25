using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace BibliotecaApi.Filters
{
    /// <summary>
    /// Filtro global de excepciones: captura errores no controlados
    /// (por ejemplo fallos de base de datos) y devuelve una respuesta
    /// 500 limpia en JSON, sin filtrar la traza al cliente.
    /// Las reglas de negocio se siguen manejando en cada controlador (400).
    /// </summary>
    public class ManejadorErroresAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            // Punto ideal para registrar context.Exception en un log.
            context.Response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new { error = "Ocurrió un error inesperado en el servidor." });
        }
    }
}
