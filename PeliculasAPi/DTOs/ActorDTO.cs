﻿using System.ComponentModel.DataAnnotations;

namespace PeliculasAPi.DTOs
{
    public class ActorDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public string Foto { get; set; }
    }
}
