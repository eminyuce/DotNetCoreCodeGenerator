using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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

     

        // use: dotnet publish --configuration Release --output ./approot
        // dotnet publish -c Release -o /root/Published
        // export ASPNETCORE_ENVIRONMENT = Development
        // echo $ASPNETCORE_ENVIRONMENT
        // dotnet publish -c Release -r ubuntu.16.04-x64
        public static IWebHost BuildWebHost(string[] args)
        {

            var appPublishedFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            var builder = WebHost.CreateDefaultBuilder(args);
           // builder.UseKestrel();
           // builder.UseContentRoot(Directory.GetCurrentDirectory());
           // builder.UseUrls("http://localhost:3083");
           // builder.UseIISIntegration();

            //appPublishedFolder = appPublishedFolder + "/publish";
         //    builder.UseContentRoot(appPublishedFolder);
            builder.UseStartup<Startup>();

          
            return builder.Build();
        }


    }
}
