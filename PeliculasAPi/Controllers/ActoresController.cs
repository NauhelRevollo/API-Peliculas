using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPi.DTOs;
using PeliculasAPi.Entidades;
using PeliculasAPi.Servicios;
using PeliculasAPi.Utilidades;

namespace PeliculasAPi.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController: ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(ApplicationDBContext context,IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery]PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadDeRegistrosPorPagina);

            var listaActores = await queryable.Paginar(paginacionDTO).ToListAsync();

            var listaActoresDTO = mapper.Map<List<ActorDTO>>(listaActores);

            return (listaActoresDTO);

        }

        [HttpGet("{id:int}", Name = "obtenerActor")]

        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var entidadActor = await context.Actores.FirstOrDefaultAsync(g => g.Id == id);

            if (entidadActor == null)
            {
                return NotFound("El actor que busca no existe");
            }

            var actorDTO = mapper.Map<ActorDTO>(entidadActor);

            return (actorDTO);
        }

        [HttpPost]

        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {

            //vamos a validar que el actor a cerar no exista con el mismo nombre
            var existeActor = await context.Actores.AnyAsync(x => x.Nombre == actorCreacionDTO.Nombre);

            if (existeActor)
            {
                return BadRequest($"Ya existe un actor con el mismo nombre: {actorCreacionDTO.Nombre}");
            }

            var entidadNueva = mapper.Map<Actor>(actorCreacionDTO);

            if (actorCreacionDTO.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    //con esto copio la foto al memoryStream
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);

                    //con esto lo convierto en un arreglo de bytes
                    var contenido = memoryStream.ToArray();

                    //recupero la extension del archivo
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);

                    //ahora completo mi entidad a guardar con una string de la url de la foto
                    entidadNueva.Foto = await almacenadorArchivos.GuardarArchivo(contenido,extension,contenedor,
                        actorCreacionDTO.Foto.ContentType);
                }

            }

            context.Add(entidadNueva);

            await context.SaveChangesAsync();

            var actorCreado = mapper.Map<ActorDTO>(entidadNueva);

            return new CreatedAtRouteResult("obtenerActor", new { id = actorCreado.Id }, actorCreado);

        }

        [HttpPut("{id}")]

        //RECORDATORIO PARA HACER LA PRUEBA EN SWAGGER CAMBIO "FromForm" por "FromBody" para poder cargar la imagen que quiero usar
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorModificado)
        {
            //vamos a validar que el actor a cerar no exista con el mismo nombre
            var actorDb = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actorDb==null)
            {
                return NotFound($"No existe el actor que quiere modificar, nombre: {actorModificado.Nombre}");
            }

            actorDb = mapper.Map(actorModificado, actorDb);

            if (actorModificado.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    //con esto copio la foto al memoryStream
                    await actorModificado.Foto.CopyToAsync(memoryStream);

                    //con esto lo convierto en un arreglo de bytes
                    var contenido = memoryStream.ToArray();

                    //recupero la extension del archivo
                    var extension = Path.GetExtension(actorModificado.Foto.FileName);

                    //ahora completo mi entidad a guardar con una string de la url de la foto
                    actorDb.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, actorDb.Foto,
                        actorModificado.Foto.ContentType);
                }

            }


            //si yo hago esto actualizo todos los campos pero puede suceder que la imagen no sea modifcada por el usuario
            //context.Entry(entidadModificada).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();

                return Ok("El actor fue modificado con exito");
            }
            catch (Exception ex)
            {

                return BadRequest($"Erro al modificar actor: " + ex.Message);
            }

        }

        [HttpPatch("{id}")]

        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument )
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var actorDb = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actorDb == null)
            {
                return NotFound($"No existe el actor que quiere modificar, Id: {id}");
            }

            var actorDTO = mapper.Map<ActorPatchDTO>(actorDb);

            patchDocument.ApplyTo(actorDTO,ModelState);

            var esValido = TryValidateModel(actorDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(actorDTO, actorDb);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existeActor = await context.Actores.AnyAsync(x => x.Id == id);

            if (!existeActor)
            {
                return NotFound($"No existe el actor que quiere eliminar, id: {id}");
            }

            context.Remove(new Actor() { Id = id });

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
