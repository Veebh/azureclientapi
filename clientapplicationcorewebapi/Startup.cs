using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;

namespace clientapplicationcorewebapi
{
    public class Startup
    {
        private readonly ILogger _logger;
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // The following will be picked up by Application Insights.
            _logger.LogInformation("Logging from ConfigureServices.");
            services.AddSwaggerGen(swaggerGenOptions =>
            {
                swaggerGenOptions.SwaggerDoc("V1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "FirstWebapp on azure",
                    Version = "V1",
                    Description = "Sample Hello world application.",
                    TermsOfService = "None."

                });

                swaggerGenOptions.OperationFilter<AddResponseHeadersFilter>();
                swaggerGenOptions.DescribeAllEnumsAsStrings();
                var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = ConfigurationPath.Combine(AppContext.BaseDirectory, xmlfile);
                if (File.Exists(xmlPath))
                {
                    swaggerGenOptions.IncludeXmlComments(xmlPath);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // The following will be picked up by Application Insights.
                _logger.LogInformation("Configuring for Development environment");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The following will be picked up by Application Insights.
                _logger.LogInformation("Configuring for Production environment");
            }
            app.UseSwagger();
            app.UseSwaggerUI(swaggerUiOptions =>
            {
                swaggerUiOptions.SwaggerEndpoint("swagger/V1/swagger.json", "helloworld");
                swaggerUiOptions.RoutePrefix = string.Empty;
                swaggerUiOptions.DefaultModelExpandDepth(-1);
            });
            app.UseStaticFiles();
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new List<string> { "index.html" }
            });
            app.UseMvc();
        }
    }
}
