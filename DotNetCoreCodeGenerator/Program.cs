using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotNetCoreCodeGenerator
{
    public class Program
    {
        //public static void Main(string[] args)
        //{
        //    BuildWebHost(args).Run();
        //}




        public static void Main(string[] args)
        {
            Console.WriteLine("Assembly.GetEntryAssembly().Location:" + 
                Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
            Console.WriteLine("AppDomain.CurrentDomain.BaseDirectory:" +
                AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("System.IO.Directory.GetCurrentDirectory:" +
                System.IO.Directory.GetCurrentDirectory());


            BuildWebHost(args).Run();
        }

        //public static IWebHost BuildWebHost(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //     .UseContentRoot(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/publish")
        //        .UseStartup<Startup>()
        //        .Build();


        //public static IWebHostBuilder CreateDefaultBuilder(string[] args)
        //{
        //    var builder = new WebHostBuilder()
        //        .UseKestrel()
        //        .UseContentRoot(Directory.GetCurrentDirectory())
        //        .ConfigureAppConfiguration((hostingContext, config) =>
        //        {
        //            var env = hostingContext.HostingEnvironment;

        //            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        //            if (env.IsDevelopment())
        //            {
        //                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
        //                if (appAssembly != null)
        //                {
        //                    config.AddUserSecrets(appAssembly, optional: true);
        //                }
        //            }

        //            config.AddEnvironmentVariables();

        //            if (args != null)
        //            {
        //                config.AddCommandLine(args);
        //            }
        //        })
        //        .ConfigureLogging((hostingContext, logging) =>
        //        {
        //            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        //            logging.AddConsole();
        //            logging.AddDebug();
        //        })
        //        .UseIISIntegration()
        //        .UseDefaultServiceProvider((context, options) =>
        //        {
        //            options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
        //        })
        //        .ConfigureServices(services =>
        //        {
        //            services.AddTransient();
        //        });

        //    return builder;
        //}

        //use: dotnet publish --configuration Release --output ./approot
        // dotnet publish -c Release -o /root/Published
        //export ASPNETCORE_ENVIRONMENT = Development
        //echo $ASPNETCORE_ENVIRONMENT
        public static IWebHost BuildWebHost(string[] args)
        {
            var appPublishedFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);


            var builder = WebHost.CreateDefaultBuilder(args);

            //appPublishedFolder = appPublishedFolder + "/publish";
         //   builder.UseContentRoot(appPublishedFolder);
         //   builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.UseStartup<Startup>();
            return builder.Build();
        }


    }
}
