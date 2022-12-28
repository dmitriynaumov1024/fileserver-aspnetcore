using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

public class TextPresentation : IFileServerPresentation
{
    public FileSizeFormatter FileSizeFormatter { get; set; } = new();

    public async Task ServeDirectory (HttpContext context, DirectoryInfo dir)
    {
        context.Response.StatusCode = 200;
        context.Response.Headers["Content-Type"] = "text/plain";

        var dirs = dir.EnumerateDirectories().OrderBy(dir => dir.Name)
            .Select(dir => $"{dir.Name}/ dir {dir.LastWriteTimeUtc:yyyy-MM-dd HH:mm:ss}\n");

        var files = dir.EnumerateFiles().OrderBy(file => file.Name)
            .Select(file => String.Format(this.FileSizeFormatter, 
                "{0} {1} {2}\n", file.Name, file.Length, file.LastWriteTimeUtc));
        
        await context.Response.WriteAsync( String.Format (
            "Current time: {0:yyyy-MM-dd HH:mm:ss}\nPath: {1}\n {2} \n{3} \n",
            DateTime.Now, context.Request.Path, 
            String.Join("", dirs), String.Join("", files)
        ));
    }

    public async Task ServeBadRequest (HttpContext context, string subheader, string text)
    {
        context.Response.StatusCode = 400;
        context.Response.Headers["Content-Type"] = "text/plain";
        await context.Response.WriteAsync ( String.Format (
            "{0} \n{1}", subheader, text
        ));
    }

    public async Task ServeForbidden (HttpContext context, string subheader, string text)
    {
        context.Response.StatusCode = 403;
        context.Response.Headers["Content-Type"] = "text/plain";
        await context.Response.WriteAsync ( String.Format (
            "{0} \n{1}", subheader, text
        ));
    }

    public async Task ServeNotFound (HttpContext context, string subheader, string text)
    {
        context.Response.StatusCode = 404;
        context.Response.Headers["Content-Type"] = "text/plain";
        await context.Response.WriteAsync ( String.Format (
            "{0} \n{1}", subheader, text
        ));
    }

    public async Task ServeInternalError (HttpContext context, string subheader, string text)
    {
        context.Response.StatusCode = 500;
        context.Response.Headers["Content-Type"] = "text/plain";
        await context.Response.WriteAsync ( String.Format (
            "{0} \n{1}", subheader, text
        ));
    }
}
