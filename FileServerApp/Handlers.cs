using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FileServerApp
{
    public static class Handlers
    {
        // Default request handler - sends index.html to client
        public static async Task DefaultGet (HttpContext context) 
        {
            await context.Response.SendFileAsync("index.html");
        }

        // File server
        public static async Task FileServer (HttpContext context) 
        {
            string rootPath = "./wwwroot/";
            // Get path from current http request
            string relPath = (string)context.Request.Query["path"] ?? String.Empty;
            // Get full path (with root path substitution)
            string path = rootPath + relPath;
            // Escaping the wwwroot directory can reveal sensitive information.
            bool goodPath = !(path.Contains("../") || path.Contains(":"));
            if (goodPath) {
                // If path represents a file - send it as attachment
                if (File.Exists(path)) {
                    await context.Response.SendFileAttachmentAsync(path);
                }
                // If path represents a directory - send information about its contents
                else if (Directory.Exists(path)) {
                    var fsEntries = FsExtensions.GetFsEntries(relPath, rootPath);
                    await context.Response.WriteAsJsonAsync(fsEntries);
                }
            }
            // Fallback
            else {
                await DefaultGet(context);
            }
        }

        // Assets delivery
        public static async Task AssetsDelivery (HttpContext context) 
        {
            string rootPath = "./assets";
            // Get path from current http request
            string relPath = (string)context.Request.Query["path"] ?? "/";
            // Get full path (with root path substitution)
            string path = rootPath + relPath;
            // Escaping the root directory can reveal sensitive information.
            bool goodPath = !(path.Contains("../") || path.Contains(":"));
            // If path is safe and file exists, send the file
            if (goodPath && File.Exists(path)) {
                await context.Response.SendFileAssetAsync(path);
            }
            // Fallback
            else {
                await context.Response.NotFound();
            }
        }
    }
}
