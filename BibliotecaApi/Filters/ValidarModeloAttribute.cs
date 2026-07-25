using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BibliotecaApi.Filters
{
    /// <summary>
    /// Filtro global: si el modelo recibido no cumple las validaciones
    /// (DataAnnotations), corta la ejecución y devuelve 400 con el detalle.
    /// </summary>
    public class ValidarModeloAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}
