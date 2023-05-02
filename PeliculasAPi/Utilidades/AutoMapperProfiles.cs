using AutoMapper;
using PeliculasAPi.DTOs;
using PeliculasAPi.Entidades;

namespace PeliculasAPi.Utilidades
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();
        }
    }
}
