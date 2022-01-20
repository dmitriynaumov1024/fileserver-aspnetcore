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
        public static async Task DefaultGet (HttpContext context) {
            await context.Response.SendFileAsync("index.html");
        }

        public static async Task FileServer (HttpContext context) {
            string relPath = (string)context.Request.Query["path"] ?? "/";
            string path = "./wwwroot/" + relPath;
            bool goodPath = !(path.Contains("../") || path.Contains(":")); 
            if (goodPath) {
                if (File.Exists(path)) {
                    await context.Response.SendFileAttachmentAsync(path);
                }
                else {
                    var fsEntries = GetFsEntries(relPath);
                    await context.Response.WriteAsJsonAsync(fsEntries);
                }
            }
            else {
                await context.Response.SendFileAsync("index.html");
            }
        }

        static Dictionary<string, object> GetFsEntries (string relPath) {
            string path = "./wwwroot" + relPath;
            IEnumerable<string> subdirs = null, files = null;
            var basedir = new DirectoryInfo(path);
            var rootdir = new DirectoryInfo("./wwwroot");
            string parent = "";

            if (basedir.Exists) {
                // Get subdirs and files
                subdirs = basedir.GetDirectories().Select(item => item.Name);
                files = basedir.GetFiles().Select(item => item.Name);
                parent = GetRelativePath(basedir.Parent, rootdir);
            }
            
            // Make a string:string dictionary
            var resultDict = new Dictionary<string, object> {
                { "base", relPath.EndsWith('/') ? relPath : relPath + '/' },
                { "parent", parent },
                { "dirs", subdirs ?? new string[0] },
                { "files", files ?? new string[0] }
            }; 

            return resultDict;
        }

        static string GetRelativePath (DirectoryInfo directory, DirectoryInfo root)
        {
            string rootPath = root.FullName.Replace('\\', '/');
            string dirPath = directory.FullName.Replace('\\', '/');
            if (rootPath.Length > dirPath.Length) {
                return "";
            }
            else {
                return dirPath.Replace(rootPath, "");
            }
        }

        static JsonSerializerOptions jsonOpts = new JsonSerializerOptions {
            WriteIndented = true,
        };

        static async Task WriteAsJsonAsync<T> (this HttpResponse context, T obj, JsonSerializerOptions options = null) 
        {
            context.ContentType = "application/json; charset=utf-8";
            await JsonSerializer.SerializeAsync(context.Body, obj, options);
        }

        static async Task SendFileAttachmentAsync (this HttpResponse response, string filepath) 
        {
            response.Headers.Append ("Content-Type", 
                "application/octet-stream");
            response.Headers.Append ("Content-Disposition",
                $"attachment; filename=\"{Path.GetFileName(filepath)}\"");
            
            await response.SendFileAsync(filepath);
        }
    }
}
