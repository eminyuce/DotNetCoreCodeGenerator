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



//Step two: Granting access to the user
//Let's say you have your WordPress server set up (running on IP address 192.168.1.100) to access a MySQL database named wordpressdb on the MySQL server with user wpadmin. On the MySQL server, you must grant access to the wordpressdb to that user from that IP address. Here's how to grant the user access(I'm assuming you already created the user wpadmin on the MySQL server and given it password %u#098Tl3).


//Log in to the MySQL server.
//Log in to MySQL with the command mysql -u root -p
//Type the MySQL root user password.
//Issue the MySQL command:
//GRANT ALL ON wordpressdb.* TO 'wpadmin'@'192.168.1.100' IDENTIFIED BY '%u#098Tl3' WITH GRANT OPTION;
//Flush the MySQL privileges with the command FLUSH PRIVILEGES;
//Exit out of the MySQL prompt with the command exit;

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
