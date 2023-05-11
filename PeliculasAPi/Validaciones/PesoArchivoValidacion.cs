using System.ComponentModel.DataAnnotations;

namespace PeliculasAPi.Validaciones
{
    public class PesoArchivoValidacion:ValidationAttribute
    {
        private readonly int pesoMaximoEnMegabytes;

        public PesoArchivoValidacion(int pesoMaximoEnMegabytes)
        {
            this.pesoMaximoEnMegabytes = pesoMaximoEnMegabytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //devuelvo success porque no estoy validando que tengamos un valor o no
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;

            //devuelvo success porque no estoy validando que tengamos el formato correcto
            if (formFile == null)
            {
                return ValidationResult.Success;
            }
            //multiplico 2 veces por 1024 para pasarlo a megabytes
            if (formFile.Length > (pesoMaximoEnMegabytes *1024 *1024))
            {
                return new ValidationResult($"El peso maximo del archivo no debe ser mayor a {pesoMaximoEnMegabytes} mb");
            }

            return ValidationResult.Success;
        }
    }
}
