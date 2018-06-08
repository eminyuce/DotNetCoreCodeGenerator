using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotNetCoreCodeGenerator.Data;
using DotNetCoreCodeGenerator.Models;
using DotNetCoreCodeGenerator.Services;
using Microsoft.Extensions.Caching.Memory;
using DotNetCodeGenerator.Domain.Services;
using DotNetCodeGenerator.Domain.Repositories;
using DotNetCodeGenerator.Domain.Helpers;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using System.IO;
using DotNetCoreCodeGenerator.Domain;
using System.Text;
using Microsoft.AspNetCore.Http;
using DbInfrastructure.EFContext;
using DbInfrastructure.Repositories;
using DbInfrastructure.Repositories.IRepositories;
using DbInfrastructure.Services.IServices;
using DbInfrastructure.Services;
using System.Reflection;

namespace DotNetCoreCodeGenerator
{
    public class Startup
    {
        private IServiceCollection _services;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static IMemoryCache _memoryCache;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(MyAppSetttings.ConnectionStringKey)));

            ;

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            // Add application services.
            services.AddSingleton<MyAppSetttings>();
            services.AddTransient<ITestEYContext>(s => new TestEYContext(Configuration.GetConnectionString("MySqlDefaultConnection")));

            AddTransientByReflection(services, typeof(IBaseService<>), "Service");
            AddTransientByReflection(services, typeof(IBaseRepository<>), "Repository");

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ITableRepository, TableRepository>();
            services.AddTransient<ITableService, TableService>();
            services.AddTransient<ICodeProducerHelper, CodeProducerHelper>();
            services.AddTransient<ISqlParserHelper, SqlParserHelper>();
            // Add application services.
            // services.AddTransient<IEmailSender, EmailSender>();
            // Add Caching Support
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddMvc();

            _services = services;

        }

        private static void AddTransientByReflection(IServiceCollection services, Type typeOfInterface, string typeofText)
        {
            var baseServiceTypes = Assembly.GetAssembly(typeOfInterface)
               .GetTypes().Where(t => t.Name.EndsWith(typeofText)).ToList();

            foreach (var type in baseServiceTypes)
            {
                string baseClass = "I" + type.Name;
                if (!baseClass.StartsWith("IBase"))
                {
                    var interfaceType = type.GetInterface(baseClass);
                    if (interfaceType != null)
                    {
                        services.AddTransient(interfaceType, type);
                    }
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            // Console.WriteLine("ContentRootPath:  " + env.ContentRootPath.Replace("DotNetCoreCodeGenerator","") + "nlog.config");
            // Console.WriteLine("WebRootPath:  " + env.WebRootPath + "nlog.config");


            if (env.IsDevelopment())
            {
                env.ConfigureNLog("nlog.config");
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                ListAllRegisteredServices(app);

            }
            else
            {
                var appPublishedFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                env.ConfigureNLog(appPublishedFolder + "/nlog.config");
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        private void ListAllRegisteredServices(IApplicationBuilder app)
        {
            app.Map("/allservices", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>All Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (var svc in _services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString());
            }));
        }
    }
}

