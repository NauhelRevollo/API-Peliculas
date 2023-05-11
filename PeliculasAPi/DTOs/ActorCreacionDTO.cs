using PeliculasAPi.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPi.DTOs
{
    public class ActorCreacionDTO: ActorPatchDTO
    {
       
        //con este tipo de datos puedo guardar archivos
        [PesoArchivoValidacion(pesoMaximoEnMegabytes:4)]
        [TipoArchivoValidacion(grupoTipoArchivo:GrupoTipoArchivo.Imagen)]
        public IFormFile Foto { get; set; }
    }
}
