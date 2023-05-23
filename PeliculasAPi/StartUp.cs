using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PeliculasAPi.Servicios;
using System.Text.Json.Serialization;

namespace PeliculasAPi
{
    public class StartUp
    {
        //Solo por un tema de comodida en la configuracion creo esta clase.
        //Actualmente no se usa y se maneja todo en la clase program.cs
        public StartUp(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //agrego para configurar automaper
            services.AddAutoMapper(typeof(StartUp));

            services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();

            //agrego para configurar la conexion a las base de datos
            services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("defaultConnection")));

            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
            //.AddNewtonsoftJson();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //agrego
            app.UseRouting();

            app.UseAuthorization();

            //agrego
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
