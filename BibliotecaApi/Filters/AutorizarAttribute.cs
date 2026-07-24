using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BibliotecaApi.Security;

namespace BibliotecaApi.Filters
{
    /// <summary>
    /// Exige un token Bearer válido. Opcionalmente restringe por rol.
    /// Uso: [Autorizar] o [Autorizar(Roles = "Bibliotecario")].
    /// Los datos del token quedan en request.Properties["TokenData"].
    /// </summary>
    public class AutorizarAttribute : AuthorizationFilterAttribute
    {
        // Lista de roles permitidos separados por coma. Vacío = cualquier usuario autenticado.
        public string Roles { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var auth = actionContext.Request.Headers.Authorization;
            if (auth == null || !auth.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(auth.Parameter))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized, "Token no proporcionado.");
                return;
            }

            var data = TokenHelper.Validate(auth.Parameter);
            if (data == null)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized, "Token inválido o expirado.");
                return;
            }

            if (!string.IsNullOrEmpty(Roles))
            {
                var permitidos = Roles.Split(',').Select(r => r.Trim());
                if (!permitidos.Contains(data.rol, StringComparer.OrdinalIgnoreCase))
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Forbidden, "No tiene permisos para esta operación.");
                    return;
                }
            }

            actionContext.Request.Properties["TokenData"] = data;
        }
    }
}
