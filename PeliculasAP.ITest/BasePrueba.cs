﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using PeliculasAPi;
using AutoMapper;
using NetTopologySuite;
using PeliculasAPi.Utilidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace PeliculasAP.ITest
{
    public class BasePrueba
    {
        protected string usuarioPorDefectoId = "9722b56a-77ea-4e41-941d-e319b6eb3712";
        protected string usuarioPorDefectoEmail = "ejemplo@hotmail.com";

        protected ApplicationDBContext ConstruirContext(string nombreDB)
        {
            var opciones = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(nombreDB).Options;

            var dbContext = new ApplicationDBContext(opciones);
            return dbContext;
        }

        protected IMapper ConfigurarAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                options.AddProfile(new AutoMapperProfiles(geometryFactory));
            });

            return config.CreateMapper();
        }

        //protected ControllerContext ConstruirControllerContext()
        //{
        //    var usuario = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.Name, usuarioPorDefectoEmail),
        //        new Claim(ClaimTypes.Email, usuarioPorDefectoEmail),
        //        new Claim(ClaimTypes.NameIdentifier, usuarioPorDefectoId)
        //    }));

        //    return new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext() { User = usuario }
        //    };
        //}

        //protected WebApplicationFactory<Startup> ConstruirWebApplicationFactory(string nombreBD,
        //    bool ignorarSeguridad = true)
        //{
        //    var factory = new WebApplicationFactory<Startup>();

        //    factory = factory.WithWebHostBuilder(builder =>
        //    {
        //        builder.ConfigureTestServices(services =>
        //        {
        //            var descriptorDBContext = services.SingleOrDefault(d =>
        //            d.ServiceType == typeof(DbContextOptions<ApplicationDBContext>));

        //            if (descriptorDBContext != null)
        //            {
        //                services.Remove(descriptorDBContext);
        //            }

        //            services.AddDbContext<ApplicationDBContext>(options =>
        //            options.UseInMemoryDatabase(nombreBD));

        //            if (ignorarSeguridad)
        //            {
        //                services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();

        //                services.AddControllers(options =>
        //                {
        //                    options.Filters.Add(new UsuarioFalsoFiltro());
        //                });
        //            }
        //        });
        //    });

        //    return factory;
        //}
    }
}