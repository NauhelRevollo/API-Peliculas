using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPi.DTOs;
using PeliculasAPi.Entidades;
using PeliculasAPi.Utilidades;

namespace PeliculasAPi.Controllers
{  
    public class GenericoController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public GenericoController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class
        {
            var listaEntidades = await context.Set<TEntidad>().AsNoTracking().ToListAsync();

            var listaDTOs = mapper.Map<List<TDTO>>(listaEntidades);

            return (listaDTOs);
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDTO paginacionDTO) where TEntidad : class
        {
            var queryable = context.Set<TEntidad>().AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadDeRegistrosPorPagina);

            var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();

            return mapper.Map<List<TDTO>>(entidades);

      

        }


        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class, IId
        {

            var entidad = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound("El recurso que busca no existe");
            }

            return mapper.Map<TDTO>(entidad);

        }

        protected async Task<ActionResult> Post<TCreacion, TEntidad, TLectura>(TCreacion creacionDTO, string nombreRuta)
            where TEntidad : class, IId
        {

            var entidadNueva = mapper.Map<TEntidad>(creacionDTO);

            context.Add(entidadNueva);

            await context.SaveChangesAsync();

            var generoCreado = mapper.Map<TLectura>(entidadNueva);

            return new CreatedAtRouteResult(nombreRuta, new { id = entidadNueva.Id }, generoCreado);

        }

        protected async Task<ActionResult> Put<TCreacion, TEntidad>(TCreacion creacionDTO, int id)
                where TEntidad : class, IId
        {
            var entidadModificada = mapper.Map<TEntidad>(creacionDTO);

            entidadModificada.Id = id;

            context.Entry(entidadModificada).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();

                return Ok("El recurso fue modificado con exito");
            }
            catch (Exception ex)
            {

                return BadRequest($"Erro al modificar recurso: " + ex.Message);
            }
        }

        protected async Task<ActionResult> Patch<TEntidad, TDTO>(int id,JsonPatchDocument<TDTO> patchDocument) 
            where TDTO : class
            where TEntidad : class, IId
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entidadDb = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);

            if (entidadDb == null)
            {
                return NotFound($"No existe el actor que quiere modificar, Id: {id}");
            }

            var entidadDTO = mapper.Map<TDTO>(entidadDb);

            patchDocument.ApplyTo(entidadDTO, ModelState);

            var esValido = TryValidateModel(entidadDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(entidadDTO, entidadDb);

            await context.SaveChangesAsync();

            return NoContent();
        }


        protected async Task<ActionResult> Delete<TEntidad>(int id)
               where TEntidad : class, IId, new()
        {
            context.Remove(new TEntidad() { Id = id });

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
