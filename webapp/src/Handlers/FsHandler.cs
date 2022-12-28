using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

public class FsHandler
{
    static FileExtensionContentTypeProvider Mime = new();

    static bool ForbiddenSymbolsIn (string path) => 
        path.Contains("//") || path.Contains("\\") || 
        path.Contains("../") || path.Contains("..\\") || 
        path.Contains(":");

    public string RootDir { get; set; }
    public IFileServerPresentation Presentation { get; set; }

    public FsHandler () 
    { 
        this.Presentation = new TextPresentation();
    }

    public virtual async Task Run (HttpContext context) 
    {
        var path = (string)context.Request.Path;

        if (path.Length > 0 && path.StartsWith("/")) { 
            path = path.Substring(1);
        }
        path = (string)Path.Combine(RootDir, path);

        if (ForbiddenSymbolsIn(context.Request.Path)) {
            await Presentation.ServeForbidden(context, "Error 403: Forbidden",
                $"Path {context.Request.Path} is not allowed.");
        }
        else if (Directory.Exists(path)) {
            await this.ServeDirectory(context, path);
        }
        else if (File.Exists(path)) {
            await this.ServeFile(context, path);
        }
        else {
            await Presentation.ServeNotFound(context, "Error 404: Not found", 
                $"No file or directory was found at {context.Request.Path}");
        }
    }

    public virtual async Task ServeDirectory (HttpContext context, string path)
    {
        try {
            await Presentation.ServeDirectory(context, new DirectoryInfo(path));
        }
        catch (DirectoryNotFoundException) {
            await Presentation.ServeNotFound(context, "Error 404: Not found", 
                $"No file or directory was found at {context.Request.Path}");
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            await Presentation.ServeInternalError(context, "Error 500: Internal server error", 
                $"Something in our server went wrong when you tried to access {context.Request.Path}");
        }
    }

    public virtual async Task ServeFile (HttpContext context, string path)
    {
        try {
            string contentType = "unknown/unknown";
            Mime.TryGetContentType(path, out contentType);
            context.Response.Headers.Append ("Content-Type", contentType);
            context.Response.Headers.Append ("Cache-Control", "public, max-age=100000000");
            await context.Response.SendFileAsync(path);
        }
        catch (FileNotFoundException) {
            await Presentation.ServeNotFound(context, "Error 404: Not found", 
                $"No file or directory was found at {context.Request.Path}");
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            await Presentation.ServeInternalError(context, "Error 500: Internal server error", 
                $"Something in our server went wrong when you tried to access {context.Request.Path}");
        }
    }
}
