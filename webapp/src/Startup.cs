using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public static string RootDirectory { get; set; }
    public static string Port { get; set; }

    public static bool RootDirectoryIsAbsolute => RootDirectory.StartsWith("/")
        || RootDirectory.Contains(":/") || RootDirectory.Contains(":\\");

    public static string AssetsRootDirectory => 
        Path.Combine(Directory.GetCurrentDirectory(), "./assets");

    public static string FsRootDirectory => RootDirectoryIsAbsolute ?
        RootDirectory : Path.Combine(Directory.GetCurrentDirectory(), RootDirectory);

    // This method gets called by the runtime. 
    // Use this method to add services to the container.
    public static void ConfigureServices (IServiceCollection services)
    {
        services.AddRouting();
    }

    // This method gets called by the runtime. 
    // Use this method to configure the HTTP request pipeline.
    public static void Configure (IApplicationBuilder app)
    {
        // Enable routing
        app.UseRouting();

        app.Use ( async (context, next) => {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context.Request.Method} {context.Request.Path}");
            await next();
        });

        var presentation = new HtmlPresentation {
            FileSizeFormatter = new () {
                SizeFormat = FileSizeFormat.Decimal
            }
        };

        var fsHandler = new FsHandler { 
            RootDir = FsRootDirectory, 
            Presentation = presentation
        };

        var downloadHandler = new DownloadHandler { 
            RootDir = FsRootDirectory,
            Presentation = presentation
        };

        app.Map ( "/fs", app => {
            app.Run (fsHandler.Run);
        });

        app.Map ( "/download", app => {
            app.Run (downloadHandler.Run);
        });

        app.UseStaticFiles ( new StaticFileOptions {
            RequestPath = "/assets",
            FileProvider = new PhysicalFileProvider(AssetsRootDirectory)
        });

        app.Run ( async (context) => {
            var path = (string)context.Request.Path;
            if (path.Length < 2) {
                context.Response.Redirect("/fs/", false, true);
            }
            else {
                await context.Response.WriteAsync($"Nothing here on {path}");
            }
        });
    }
}
