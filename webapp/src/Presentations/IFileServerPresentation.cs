using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

public interface IFileServerPresentation
{
    Task ServeDirectory (HttpContext context, DirectoryInfo dir);
    Task ServeBadRequest (HttpContext context, string subheader, string text);
    Task ServeForbidden (HttpContext context, string subheader, string text);
    Task ServeNotFound (HttpContext context, string subheader, string text); 
    Task ServeInternalError (HttpContext context, string subheader, string text);
}
