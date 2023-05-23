using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PeliculasAPi.DTOs;
using PeliculasAPi.Entidades;
using PeliculasAPi.Servicios;
using PeliculasAPi.Utilidades;
using System.Linq.Dynamic.Core;

namespace PeliculasAPi.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController: GenericoController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<PeliculasController> logger;
        private readonly string contenedor = "peliculas";

        public PeliculasController(ApplicationDBContext context,IMapper mapper,IAlmacenadorArchivos almacenadorArchivos,
            ILogger<PeliculasController> logger)
            : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDTO>> Get()
        {
            var top = 5;
            var fechaActual = DateTime.Today;

            //esta lista devuelve los proximo estrenos 
            var listaProximosEstrenos = await context.Peliculas.
                Where(p => p.FechaEstreno>fechaActual).
                OrderBy(p => p.FechaEstreno).
                Take(top).
                ToListAsync();

            //esta lista devuelve las peliculas que estan en salas de cine actualmente
            var listaEnCines = await context.Peliculas.
               Where(p => p.EnCines).               
               Take(top).
               ToListAsync();

            //con esta linea traigo todos los resultados sin filtrar
            //var listaPeliculas = await context.Peliculas.ToListAsync();

            var resultado = new PeliculasIndexDTO();
            resultado.FuturosEstrenos= mapper.Map<List<PeliculaDTO>>(listaProximosEstrenos);
            resultado.EnCines= mapper.Map<List<PeliculaDTO>>(listaEnCines);            

            return (resultado);
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculaDTO filtroPeliculasDTO)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculasDTO.Titulo));
            }

            if (filtroPeliculasDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
            }

            if (filtroPeliculasDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > hoy);
            }

            if (filtroPeliculasDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                    .Contains(filtroPeliculasDTO.GeneroId));
            }

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.CampoOrdenar))
            {
                var tipoOrden = filtroPeliculasDTO.OrdenAscendente ? "ascending" : "descending";

                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculasDTO.CampoOrdenar} {tipoOrden}");

                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                }
            }

            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable,
                filtroPeliculasDTO.CantidadRegistrosPorPagina);

            var peliculas = await peliculasQueryable.Paginar(filtroPeliculasDTO.Paginacion).ToListAsync();

            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("{id:int}", Name = "obtenerPelicula")]

        public async Task<ActionResult<PeliculaDetalleDTO>> Get(int id)
        {
            var entidadPelicula = await context.Peliculas.
                Include(g => g.PeliculasGeneros).ThenInclude(g => g.Genero).
                Include(g => g.PeliculasActores).ThenInclude(g => g.Actor).
                FirstOrDefaultAsync(g => g.Id == id);

            if (entidadPelicula == null)
            {
                return NotFound("La pelicula que busca no existe");
            }

            entidadPelicula.PeliculasActores = entidadPelicula.PeliculasActores.OrderBy(g => g.Orden).ToList();

            var peliculaDTO = mapper.Map<PeliculaDetalleDTO>(entidadPelicula);

            return (peliculaDTO);
        }

        [HttpPost]

        //RECORDATORIO PARA HACER LA PRUEBA EN SWAGGER CAMBIO "FromForm" por "FromBody" para poder cargar la imagen que quiero usar
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {

            //vamos a validar que el actor a cerar no exista con el mismo nombre
            var existePelicula = await context.Peliculas.AnyAsync(x => x.Titulo == peliculaCreacionDTO.Titulo);

            if (existePelicula)
            {
                return BadRequest($"Ya existe una pelicula con el mismo nombre: {peliculaCreacionDTO.Titulo}");
            }

            var entidadNueva = mapper.Map<Pelicula>(peliculaCreacionDTO);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    //con esto copio la foto al memoryStream
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);

                    //con esto lo convierto en un arreglo de bytes
                    var contenido = memoryStream.ToArray();

                    //recupero la extension del archivo
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);

                    //ahora completo mi entidad a guardar con una string de la url de la foto
                    entidadNueva.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor,
                        peliculaCreacionDTO.Poster.ContentType);
                }

            }

            AsignarOrdenActores(entidadNueva);

            context.Add(entidadNueva);

            await context.SaveChangesAsync();

            var peliculaCreada = mapper.Map<PeliculaDTO>(entidadNueva);

            return new CreatedAtRouteResult("obtenerPelicula", new { id = peliculaCreada.Id }, peliculaCreada);

        }

        [HttpPut("{id}")]

        //RECORDATORIO PARA HACER LA PRUEBA EN SWAGGER CAMBIO "FromForm" por "FromBody" para poder cargar la imagen que quiero usar
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaModificada)
        {
            //vamos a validar que el actor a cerar no exista con el mismo nombre
            var peliculaDb = await context.Peliculas.Include(x => x.PeliculasActores).Include(x => x.PeliculasGeneros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (peliculaDb == null)
            {
                return NotFound($"No existe la pelicula que quiere modificar, titulo: {peliculaModificada.Titulo}");
            }

            peliculaDb = mapper.Map(peliculaModificada, peliculaDb);

            if (peliculaModificada.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    //con esto copio la foto al memoryStream
                    await peliculaModificada.Poster.CopyToAsync(memoryStream);

                    //con esto lo convierto en un arreglo de bytes
                    var contenido = memoryStream.ToArray();

                    //recupero la extension del archivo
                    var extension = Path.GetExtension(peliculaModificada.Poster.FileName);

                    //ahora completo mi entidad a guardar con una string de la url de la foto
                    peliculaDb.Titulo = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, peliculaDb.Titulo,
                        peliculaModificada.Poster.ContentType);
                }

            }


            //si yo hago esto actualizo todos los campos pero puede suceder que la imagen no sea modifcada por el usuario
            //context.Entry(entidadModificada).State = EntityState.Modified;

            AsignarOrdenActores(peliculaDb);
            try
            {
                await context.SaveChangesAsync();

                return Ok("La pelicula fue modificada con exito");
            }
            catch (Exception ex)
            {

                return BadRequest($"Erro al modificar pelicula: " + ex.Message);
            }

        }

        [HttpPatch("{id}")]

        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
        {
            return await Patch<Pelicula, PeliculaPatchDTO>(id, patchDocument);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existePelicula = await context.Peliculas.AnyAsync(x => x.Id == id);

            if (!existePelicula)
            {
                return NotFound($"No existe la pelicula que quiere eliminar, id: {id}");
            }

            return await Delete<Pelicula>(id);
        }

        private void AsignarOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActores != null)
            {
                for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }
    }
}
