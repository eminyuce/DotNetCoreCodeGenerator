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

namespace DotNetCoreCodeGenerator
{
    public class Startup
    {
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
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            // Add application services.
            services.AddSingleton<MyAppSetttings>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ITableRepository, TableRepository>();
            services.AddTransient<ITableService, TableService>();
            services.AddTransient<ICodeProducerHelper, CodeProducerHelper>();
            services.AddTransient<ISqlParserHelper, SqlParserHelper>();
            // Add application services.
            // services.AddTransient<IEmailSender, EmailSender>();
            // Add Caching Support
            services.AddMemoryCache();
            services.AddMvc();
         


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
            }
            else
            {
                var appPublishedFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
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
    }
}

