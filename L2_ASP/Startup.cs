using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace L2_ASP
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<CompanyService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var companyService = context.RequestServices.GetService<CompanyService>();

                    await context.Response.WriteAsync($"Company with the most employees: {companyService.GetCompanyWithMostEmployees()}\n");

                    await context.Response.WriteAsync($"Your name: {Configuration["Name"]}\n");
                    await context.Response.WriteAsync($"Your age: {Configuration["Age"]}\n");
                    await context.Response.WriteAsync($"Your location: {Configuration["Location"]}\n");
                });
            });
        }
    }

    public class Company
    {
        public string Name { get; set; }
        public int Employees { get; set; }
    }

    public class CompanyService
    {
        private readonly IConfiguration _configuration;

        public CompanyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetCompanyWithMostEmployees()
        {
            var companies = new List<Company>();

            // Get companies from different configuration sources
            companies.AddRange(_configuration.GetSection("Companies").Get<List<Company>>());
            companies.AddRange(_configuration.GetSection("Companies").Get<List<Company>>());
            companies.AddRange(_configuration.GetSection("Companies").Get<List<Company>>());

            var companyWithMostEmployees = companies.OrderByDescending(c => c.Employees).FirstOrDefault();

            return companyWithMostEmployees != null ? companyWithMostEmployees.Name : "No company found";
        }
    }
}
