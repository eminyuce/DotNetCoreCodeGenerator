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
            //Console.WriteLine("Assembly.GetEntryAssembly().Location:" + Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
            //Console.WriteLine("AppDomain.CurrentDomain.BaseDirectory:" + AppDomain.CurrentDomain.BaseDirectory);
            //Console.WriteLine("System.IO.Directory.GetCurrentDirectory:" + System.IO.Directory.GetCurrentDirectory());


            BuildWebHost(args).Run();
        }

        //public static IWebHost BuildWebHost(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //     .UseContentRoot(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/publish")
        //        .UseStartup<Startup>()
        //        .Build();

        public static IWebHost BuildWebHost(string[] args) =>
     WebHost.CreateDefaultBuilder(args)
         .UseStartup<Startup>()
         .Build();

    }
}
