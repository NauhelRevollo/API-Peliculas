﻿using PeliculasAPi.DTOs;
using PeliculasAPi.Utilidades;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPi.Entidades
{
    public class Pelicula : IId
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        public string Poster { get; set; }

        public List<PeliculasActores> PeliculasActores { get; set; }

        public List<PeliculasGeneros> PeliculasGeneros { get; set; }
    }
}