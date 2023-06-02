using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPi.Controllers;
using PeliculasAPi.DTOs;
using PeliculasAPi.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAP.ITest.PruebasUnitarias
{
    [TestClass]
    public class GenerosControllerTest : BasePrueba
    {
        [TestMethod]

        public async Task ObtenerTodosLosGeneros()
        {
            //con esta linea creo un nuevo nombre aleatorio para la
            //base de datos y asi no se repite con otra base de otra prueba
            var nombreDB = Guid.NewGuid().ToString();

            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            contexto.Generos.Add(new Genero() { Nombre = "Género 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);

            
            var controller = new GenerosController(contexto2, mapper);
            var respuesta = await controller.Get();

            
            var generos = respuesta.Value;
            Assert.AreEqual(2, generos.Count);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdNoExistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controller = new GenerosController(contexto, mapper);
            var respuesta = await controller.Get(1);

            var resultado = respuesta.Result as StatusCodeResult;

            if (resultado !=null)
            {
                Assert.AreEqual(404, resultado.StatusCode);

            }
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdExistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
            contexto.Generos.Add(new Genero() { Nombre = "Genero 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);
            var controller = new GenerosController(contexto2, mapper);

            var id = 1;
            var respuesta = await controller.Get(id);
            var resultado = respuesta.Value;
            Assert.AreEqual(id, resultado.Id);
        }

        [TestMethod]
        public async Task CrearGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var nuevoGenero = new GeneroCreacionDTO() { Nombre = "nuevo género" };

            var controller = new GenerosController(contexto, mapper);

            var respuesta = await controller.Post(nuevoGenero);
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);

            var contexto2 = ConstruirContext(nombreBD);
            var cantidad = await contexto2.Generos.CountAsync();
            Assert.AreEqual(1, cantidad);
        }

        [TestMethod]
        public async Task ActualizarGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);
            var controller = new GenerosController(contexto2, mapper);

            var generoCreacionDTO = new GeneroCreacionDTO() { Nombre = "Nuevo nombre" };

            var id = 1;
            var respuesta = await controller.Put(id, generoCreacionDTO);

            var resultado = respuesta as StatusCodeResult;


            if (resultado != null)
            {
                Assert.AreEqual(204, resultado.StatusCode);

            }

           
            var contexto3 = ConstruirContext(nombreBD);
            var existe = await contexto3.Generos.AnyAsync(x => x.Nombre == "Nuevo nombre");
            Assert.IsTrue(existe);
        }

        [TestMethod]
        public async Task IntentaBorrarGeneroNoExistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controller = new GenerosController(contexto, mapper);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;

            if (resultado != null)
            {
                Assert.AreEqual(404, resultado.StatusCode);

            }

            
        }

        [TestMethod]
        public async Task BorrarGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);
            var controller = new GenerosController(contexto2, mapper);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreBD);
            var existe = await contexto3.Generos.AnyAsync();
            Assert.IsFalse(existe);
        }

    }
}
