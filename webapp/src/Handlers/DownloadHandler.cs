using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class DownloadHandler : FsHandler
{
    public DownloadHandler () : base () { }

    public override async Task ServeDirectory (HttpContext context, string path)
    {
        await Presentation.ServeBadRequest(context, "Error 400: Bad request", 
            $"Directories like {context.Request.Path} can not be downloaded at the moment.");
        }

    public override async Task ServeFile (HttpContext context, string path)
    {
        try {
            context.Response.Headers.Append ("Content-Type", "application/octet-stream");
            context.Response.Headers.Append ("Content-Disposition", $"attachment; filename=\"{Path.GetFileName(path)}\"");
            await context.Response.SendFileAsync(path);
        }
        catch (FileNotFoundException) {
            await Presentation.ServeNotFound(context, "Error 404: Not found", 
                $"No file or directory was found at {context.Request.Path}");
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            await Presentation.ServeInternalError(context, "Error 500: Internal server error", 
                $"Something in our server went wrong when you tried to download {context.Request.Path}");
        }
    }
}
