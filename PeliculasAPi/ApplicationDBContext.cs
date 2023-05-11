using Microsoft.EntityFrameworkCore;
using PeliculasAPi.Entidades;

namespace PeliculasAPi
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet <Genero> Generos { get; set; }
        public DbSet <Actor> Actores { get; set; }
    }
}
