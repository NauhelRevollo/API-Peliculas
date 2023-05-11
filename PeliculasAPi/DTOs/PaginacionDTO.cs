namespace PeliculasAPi.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;

        private int cantidadDeRegistrosPorPagina { get; set; } = 10;

        private readonly int cantidadMaximaRegistrosPorPagina = 50;

        public int CantidadDeRegistrosPorPagina
        {
            get=> cantidadDeRegistrosPorPagina;
            set
            {
                cantidadDeRegistrosPorPagina = (value > cantidadMaximaRegistrosPorPagina)? cantidadMaximaRegistrosPorPagina: value;
            }
        }
    }
}
