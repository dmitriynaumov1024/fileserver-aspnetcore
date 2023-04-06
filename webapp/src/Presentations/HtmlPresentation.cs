using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

public class HtmlPresentation : IFileServerPresentation
{
    static string 
        ErrorView = File.ReadAllText("assets/html/ErrorView.html"),
        FsView = File.ReadAllText("assets/html/FsView.html"),
        DirectoryWidget = File.ReadAllText("assets/html/DirectoryWidget.html"),
        FileWidget = File.ReadAllText("assets/html/FileWidget.html"),
        BreadcrumbWidget = File.ReadAllText("assets/html/BreadcrumbWidget.html");

    public FileSizeFormatter FileSizeFormatter { get; set; } = new();

    static string UriEscape (string text) => Uri.EscapeDataString(text);

    public async Task ServeDirectory (HttpContext context, DirectoryInfo dir)
    {
        context.Response.StatusCode = 200;
        context.Response.Headers["Content-Type"] = "text/html";

        var dirsFormatted = dir.EnumerateDirectories()
            .OrderBy(dir => dir.Name)
            .Select(dir => String.Format (DirectoryWidget, 
                Path.Join("/fs", context.Request.Path, UriEscape(dir.Name)), 
                dir.Name, 
                dir.LastWriteTimeUtc
            )).ToList();

        var filesFormatted = dir.EnumerateFiles()
            .OrderBy(file => file.Name)
            .Select(file => String.Format(FileWidget,
                Path.Join("/fs", context.Request.Path, UriEscape(file.Name)), 
                Path.Join("/download", context.Request.Path, UriEscape(file.Name)), 
                file.Name, 
                this.FileSizeFormatter.Format("", file.Length, null), 
                file.LastWriteTimeUtc
            )).ToList();

        var pathParts = ((string)context.Request.Path)
            .Split("/", StringSplitOptions.RemoveEmptyEntries);
        
        var breadcrumbs = new (string name, string link)[pathParts.Length>0 ? pathParts.Length : 1];
        
        breadcrumbs[0] = ("", "/fs/");
        for (int i=1; i<breadcrumbs.Length; i++) {
            var part = pathParts[i-1];
            breadcrumbs[i] = (part, breadcrumbs[i-1].link + UriEscape(part) + "/");
        }

        // to set up a parent directory link
        if (pathParts.Length > 0) {
            dirsFormatted.Insert(0, String.Format(DirectoryWidget, 
                breadcrumbs[breadcrumbs.Length - 1].link,
                "..",
                null
            ));
        }

        var breadcrumbsFormatted = breadcrumbs
            .Select(b => String.Format(BreadcrumbWidget, b.link, b.name));

        await context.Response.WriteAsync ( String.Format ( 
            FsView,
            context.Request.Path + " - File server",
            pathParts.Length > 0 ? dir.Name : "Index of /",
            String.Join("", breadcrumbsFormatted),
            String.Join("\n", dirsFormatted),
            String.Join("\n", filesFormatted)
        ));
    }

    public async Task ServeBadRequest (HttpContext context, string subheader, string text)
    {
        context.Response.StatusCode = 400;
        context.Response.Headers["Content-Type"] = "text/html";
        await context.Response.WriteAsync ( String.Format (
            ErrorView, "Bad request", subheader, text 
        ));
    }

    public async Task ServeForbidden (HttpContext context, string subheader, string text)
    {
        context.Response.StatusCode = 403;
        context.Response.Headers["Content-Type"] = "text/html";
        await context.Response.WriteAsync ( String.Format (
            ErrorView, "Forbidden", subheader, text 
        ));
    }

    public async Task ServeNotFound (HttpContext context, string subheader, string text)
    {
        context.Response.StatusCode = 404;
        context.Response.Headers["Content-Type"] = "text/html";
        await context.Response.WriteAsync ( String.Format (
            ErrorView, "Not found", subheader, text 
        ));
    }

    public async Task ServeInternalError (HttpContext context, string subheader, string text)
    {
        context.Response.StatusCode = 500;
        context.Response.Headers["Content-Type"] = "text/html";
        await context.Response.WriteAsync ( String.Format (
            ErrorView, "Internal error", subheader, text 
        ));
    }
}
