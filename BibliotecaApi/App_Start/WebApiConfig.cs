using BibliotecaApi.Filters;
using BibliotecaApi.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BibliotecaApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configurar CORS (única fuente; Web API maneja también el preflight OPTIONS)
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Filtros globales: validación de modelo y manejo de errores no controlados
            config.Filters.Add(new ManejadorErroresAttribute());
            config.Filters.Add(new ValidarModeloAttribute());

            // Configuración y servicios de Web API

            // Rutas de Web API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Configurar para que la API regrese JSON por defecto
            var jsonFormatter = config.Formatters.JsonFormatter;
            config.Formatters.Clear();
            config.Formatters.Add(jsonFormatter);
        }
    }
}