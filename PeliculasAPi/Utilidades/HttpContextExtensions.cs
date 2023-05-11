using Microsoft.EntityFrameworkCore;

namespace PeliculasAPi.Utilidades
{
    public static class HttpContextExtensions
    {
        public async  static Task InsertarParametrosPaginacion<T>(this HttpContext httpContext, IQueryable<T> queryable
            ,int cantidadRegistrosPorPaginas)
        {
            double cantidad = await queryable.CountAsync();
            double cantidadPaginas = Math.Ceiling(cantidad/ cantidadRegistrosPorPaginas);

            httpContext.Response.Headers.Add("cantidadPaginas", cantidadPaginas.ToString());

        }
    }
}
