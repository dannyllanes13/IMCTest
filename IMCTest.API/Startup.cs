using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IMCTest.Service.Implementation;
using IMCTest.Service.Interface;
using IMCTest.Service.MapConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Remoting;

namespace IMCTest.API
{
    public class Startup
    {
        public delegate ITaxCalculator ServiceResolver(string key);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MapProfile));

            services.AddTransient<TaxCalculatorClientA>();
            services.AddTransient<TaxCalculatorClientB>();


            services.AddTransient<ServiceResolver>(serviceProvider => key =>
            {
                //here match the clientId with the implementation i have on my appsettings.json
                //this avoid to republish if a newe client has same existing implementation
                var implementation = Configuration.GetValue<string>(key);

                switch (implementation)
                {
                    case "ImplementationA":
                        return serviceProvider.GetService<TaxCalculatorClientA>();
                    case "ImplementationB":
                        return serviceProvider.GetService<TaxCalculatorClientB>();
                    default:
                        throw new KeyNotFoundException();
                }
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
