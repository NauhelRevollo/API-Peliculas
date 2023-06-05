using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPi.DTOs;
using PeliculasAPi.Entidades;
using System.Diagnostics.Metrics;
using System.Runtime.ConstrainedExecution;

namespace PeliculasAPi.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController : GenericoController
    {
       
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDBContext context, IMapper mapper):base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get()
        {
            //var listaGeneros = await context.Generos.ToListAsync();

            //var listaGenerosDTO = mapper.Map<List<GeneroDTO>>(listaGeneros);

            //return (listaGenerosDTO);

            return await Get<Genero, GeneroDTO>();

        }

        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            //var entidadGenero = await context.Generos.FirstOrDefaultAsync(g => g.Id == id);

            //if (entidadGenero == null)
            //{
            //    return NotFound("El genero que busca no existe");
            //}

            //var genetoDTO = mapper.Map<GeneroDTO>(entidadGenero);

            //return (genetoDTO);

            return await Get<Genero, GeneroDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {

            //vamos a validar que el genero a cerar no exista con el mismo nombre
            var existeGenero = await context.Generos.AnyAsync(x => x.Nombre == generoCreacionDTO.Nombre);

            if (existeGenero)
            {
                return BadRequest($"Ya existe un genero con el mismo nombre: {generoCreacionDTO.Nombre}");
            }

            //var entidadNueva = mapper.Map<Genero>(generoCreacionDTO);

            //context.Add(entidadNueva);

            //await context.SaveChangesAsync();

            //var generoCreado = mapper.Map<GeneroDTO>(entidadNueva);

            //return new CreatedAtRouteResult("obtenerGenero", new { id = generoCreado.Id }, generoCreado);

            return await Post<GeneroCreacionDTO,Genero, GeneroDTO>(generoCreacionDTO, "obtenerGenero");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoModificado)
        {
            //vamos a validar que el genero a cerar no exista con el mismo nombre
            var existeGenero = await context.Generos.AnyAsync(x => x.Id == id);

            if (!existeGenero)
            {
                return NotFound($"No existe el genero que quiere modificar, nombre: {generoModificado.Nombre}");
            }

            //var entidadModificada = mapper.Map<Genero>(generoModificado);

            //entidadModificada.Id = id;

            //context.Entry(entidadModificada).State = EntityState.Modified;

            //try
            //{
            //    await context.SaveChangesAsync();

            //    return Ok("El genero fue modificado con exito");
            //}
            //catch (Exception ex)
            //{

            //    return BadRequest($"Erro al modificar genero: " + ex.Message);
            //}

            return await Put<GeneroCreacionDTO, Genero>(generoModificado, id);

        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles ="Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var existeGenero = await context.Generos.AnyAsync(x => x.Id == id);

            if (!existeGenero)
            {
                return NotFound($"No existe el genero que quiere eliminar, id: {id}");
            }

            //context.Remove(new Genero() { Id=id});

            //await context.SaveChangesAsync();

            //return NoContent();

            return await Delete<Genero>(id);
        }

    }
}
