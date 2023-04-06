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
        string requestPath = Uri.UnescapeDataString(context.Request.Path);
        try {
            string filename = Uri.EscapeDataString(Path.GetFileName(path));
            context.Response.Headers["Content-Type"] = "application/octet-stream";
            context.Response.Headers["Content-Disposition"] = $"attachment; filename*=utf-8''{filename}";
            await context.Response.SendFileAsync(path);
        }
        catch (FileNotFoundException) {
            await Presentation.ServeNotFound(context, "Error 404: Not found", 
                $"No file or directory was found at {requestPath}");
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            await Presentation.ServeInternalError(context, "Error 500: Internal server error", 
                $"Something went wrong while trying to download {requestPath}");
        }
    }
}
