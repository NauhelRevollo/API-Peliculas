using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PeliculasAPi.Utilidades
{
    public class PeliculaExisteAttribute : Attribute, IAsyncResourceFilter
    
    {
        private readonly ApplicationDBContext Dbcontext;

        public PeliculaExisteAttribute(ApplicationDBContext context)
        {
            this.Dbcontext = context;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
            ResourceExecutionDelegate next)
        {
            var peliculaIdObject = context.HttpContext.Request.RouteValues["peliculaId"];

            if (peliculaIdObject == null)
            {
                return;
            }

            var peliculaId = int.Parse(peliculaIdObject.ToString());

            var existePelicula = await Dbcontext.Peliculas.AnyAsync(x => x.Id == peliculaId);

            if (!existePelicula)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                await next();
            }
        }
    }
}
