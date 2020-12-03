



using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ParkyAPI.Data;
using ParkyAPI.Models.ParkyMapper;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ParkDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ParkyAPI")));
            services.AddScoped<INationalParkRepository, NationalParkRepository>();

            services.AddAutoMapper(typeof(ParkyMappings));
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ParkyOpenAPISpec", new OpenApiInfo()
                {
                    Title = "Parky API",
                    Version = "1"
                });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/ParkyOpenAPISpec/swagger.json", "Parky API");
                    options.RoutePrefix = "";
                }
            );
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
