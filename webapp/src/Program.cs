using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main (string[] args)
    {
        if (args.Length < 2) {
            Console.WriteLine("Expected arguments: <root-directory> <port>");
            return;
        }

        Startup.RootDirectory = args[0];
        Startup.Port = args[1];

        // create app
        var app = new WebHostBuilder()
            .UseKestrel()
            .ConfigureServices(Startup.ConfigureServices)
            .Configure(Startup.Configure)
            .UseUrls($"http://0.0.0.0:{Startup.Port}")
            .Build();
        
        // serve
        app.Run();
    }
}
