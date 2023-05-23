using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PeliculasAPi.Utilidades;
using PeliculasAPi.Validaciones;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PeliculasAPi.DTOs
{
    public class PeliculaCreacionDTO: PeliculaPatchDTO
    {        

        [PesoArchivoValidacion(pesoMaximoEnMegabytes: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Poster { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIDs { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculaCreacionDTO>>))]
        public List<ActorPeliculaCreacionDTO> Actores { get; set; }
    }
}
