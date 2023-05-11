using PeliculasAPi.DTOs;

namespace PeliculasAPi.Utilidades
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable.
                Skip((paginacionDTO.Pagina - 1) * paginacionDTO.CantidadDeRegistrosPorPagina).
                Take(paginacionDTO.CantidadDeRegistrosPorPagina);
        }
    }
}
