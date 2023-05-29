﻿using AutoMapper;
using NetTopologySuite.Geometries;
using PeliculasAPi.DTOs;
using PeliculasAPi.Entidades;

namespace PeliculasAPi.Utilidades
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<SalaDeCine, SalaDeCineDTO>()
               .ForMember(x => x.Latitud, options => options.MapFrom(x => x.Ubicacion.Y))
               .ForMember(x => x.Longitud, options => options.MapFrom(x => x.Ubicacion.X));

            CreateMap<SalaDeCineDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion, options => options.MapFrom(y =>
                geometryFactory.CreatePoint(new Coordinate(y.Longitud,y.Latitud))));
               


            CreateMap<SalaDeCineCreacionDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion, options => options.MapFrom(x =>
                geometryFactory.CreatePoint(new Coordinate(x.Longitud, x.Latitud))));


            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto,options => options.Ignore());

            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<PeliculaCreacionDTO, Pelicula>()
               .ForMember(x => x.Poster, options => options.Ignore())
               .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
               .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActores));

            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();


            CreateMap<Pelicula, PeliculaDetalleDTO>().
                ForMember(x => x.Generos, options => options.MapFrom(MapPeliculasGeneros)).
                ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));




        }

        private List<ActorPeliculaDetalleDTO> MapPeliculasActores(Pelicula pelicula, PeliculaDetalleDTO peliculaDetalleDTO)
        {
            var resultado = new List<ActorPeliculaDetalleDTO>();

            if (pelicula.PeliculasActores == null)
            {
                return resultado;
            }

            foreach (var actorPelicula in pelicula.PeliculasActores)
            {
                resultado.Add(new ActorPeliculaDetalleDTO()
                { ActorId = actorPelicula.ActorId, Nombre = actorPelicula.Actor.Nombre,Personaje= actorPelicula.Personaje });
            }

            return resultado;
        }

        private List<GeneroDTO> MapPeliculasGeneros( Pelicula pelicula, PeliculaDetalleDTO peliculaDetalleDTO)
        {
            var resultado = new List<GeneroDTO>();

            if (pelicula.PeliculasGeneros == null)
            {
                return resultado;
            }

            foreach (var generoPelicula in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroDTO() { Id = generoPelicula.GeneroId, Nombre= generoPelicula.Genero.Nombre });
            }

            return resultado;
        }

        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO,Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();

            if (peliculaCreacionDTO.GenerosIDs == null)
            {
                return resultado;
            }

            foreach (var id in peliculaCreacionDTO.GenerosIDs)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId=id});
            }

            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();

            if (peliculaCreacionDTO.Actores == null)
            {
                return resultado;
            }

            foreach (var actor in peliculaCreacionDTO.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.ActorId,Personaje=actor.Personaje});
            }

            return resultado;
        }
    }
}
