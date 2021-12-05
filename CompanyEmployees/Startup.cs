using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using AutoMapper;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.Extensions;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Repository;
using Repository.DataShaping;


namespace CompanyEmployees
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors(); //Config headers
            services.ConfigureIISIntegration(); // config server 
            services.ConfigureLoggerService();// add config log
            services.ConfigureSqlContext(Configuration); // add DbContext in DI container
            services.ConfigureRepositoryManager(); // add Repository in DI container
            services.AddAutoMapper(typeof(Startup)); // add AutoMapper 
            services.AddScoped<ValidationFilterAttribute>();
            services.AddScoped<ValidateEmployeeForCompanyExistsAttribute>();
            services.AddScoped<IDataShaper<EmployeeDTO>, DataShaper<EmployeeDTO>>();
            services.AddAuthentication(); 
            services.ConfigureIdentity();
            services.ConfigureJWT(Configuration);
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true; 
                //suppress a default model state validation that is implemented due to the existence of 
                //[ApiController].Before our request reaches the action, it is validated with the [ApiController]
            });
            services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CompanyEmployees", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CompanyEmployees v1"));
            }
            else
            {
                app.UseHsts();
            }
            app.ConfigureExceptionHandler(logger);
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
