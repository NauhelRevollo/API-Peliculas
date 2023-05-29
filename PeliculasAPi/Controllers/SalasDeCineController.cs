using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PeliculasAPi.DTOs;
using PeliculasAPi.Entidades;
using PeliculasAPi.Servicios;

namespace PeliculasAPi.Controllers
{
    [ApiController]
    [Route("api/salasDeCine")]
    public class SalasDeCineController : GenericoController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly GeometryFactory geometryFactory;

        public SalasDeCineController(ApplicationDBContext context, IMapper mapper,GeometryFactory geometryFactory)
        : base(context, mapper)
        {
            this.mapper = mapper;
            this.geometryFactory = geometryFactory;
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineDTO>>> Get()
        {

            return await Get<SalaDeCine, SalaDeCineDTO>();

        }

        [HttpGet("{id:int}", Name = "obtenerSalaDeCine")]

        public async Task<ActionResult<SalaDeCineDTO>> Get(int id)
        {

            return await Get<SalaDeCine, SalaDeCineDTO>(id);

        }

        [HttpGet("Cercanos")]

        public async Task<ActionResult<List<SalaDeCineCercanoDTO>>> Cercanos
            ([FromQuery] SalaDeCineCercanoFiltroDTO filtro)
        {
            var ubicacionUsuario = geometryFactory.CreatePoint(new Coordinate(filtro.Longitud, filtro.Latitud));

            var salasDeCine = await context.SalasDeCine
                .OrderBy(x => x.Ubicacion.Distance(ubicacionUsuario))
                .Where(x => x.Ubicacion.IsWithinDistance(ubicacionUsuario, filtro.DistanciaEnKms * 1000))
                .Select(x => new SalaDeCineCercanoDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Latitud = x.Ubicacion.Y,
                    Longitud = x.Ubicacion.X,
                    DistanciaEnMetros = Math.Round(x.Ubicacion.Distance(ubicacionUsuario))
                })
                .ToListAsync();

            return salasDeCine;

        }



        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {

            
            var existeSala = await context.SalasDeCine.AnyAsync(x => x.Nombre == salaDeCineCreacionDTO.Nombre);

            if (existeSala)
            {
                return BadRequest($"Ya existe una sala con el mismo nombre: {salaDeCineCreacionDTO.Nombre}");
            }            

            return await Post<SalaDeCineCreacionDTO, SalaDeCine, SalaDeCineDTO>(salaDeCineCreacionDTO, "obtenerSalaDeCine");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] SalaDeCineCreacionDTO salaModificada)
        {

            var existeSala = await context.SalasDeCine.AnyAsync(x => x.Id == id);

            if (!existeSala)
            {
                return BadRequest($"No existe el recurso: {id}");
            }

            return await Put<SalaDeCineCreacionDTO, SalaDeCine>(salaModificada, id);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existeSala = await context.SalasDeCine.AnyAsync(x => x.Id == id);

            if (!existeSala)
            {
                return BadRequest($"No existe una sala numero: {id}");
            }            

            return await Delete<SalaDeCine>(id);
        }
    }
}
