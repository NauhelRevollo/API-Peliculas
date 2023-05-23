using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using PeliculasAPi.DTOs;
using System.ComponentModel;

namespace PeliculasAPi.Utilidades
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync( ModelBindingContext bindingContext)
        {
            var nombrePropiedad = bindingContext.ModelName;

            var  proveedorValores = bindingContext.ValueProvider.GetValue(nombrePropiedad);


            if (proveedorValores == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            try
            {
                var valorDeserializado = JsonConvert.DeserializeObject<T>(proveedorValores.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(valorDeserializado);
            }
            catch
            {

                bindingContext.ModelState.TryAddModelError(nombrePropiedad, $"Error, valor invalido para tipo");
            }

            return Task.CompletedTask;

        }
    }
}
