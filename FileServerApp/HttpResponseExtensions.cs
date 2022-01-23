using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace FileServerApp
{
    public static class HttpResponseExtensions
    {
        public static async Task WriteAsJsonAsync<T> (this HttpResponse context, T obj, JsonSerializerOptions options = null) 
        {
            context.ContentType = "application/json; charset=utf-8";
            await JsonSerializer.SerializeAsync(context.Body, obj, options);
        }

        public static async Task SendFileAttachmentAsync (this HttpResponse response, string filepath) 
        {
            response.Headers.Append ("Content-Type", 
                "application/octet-stream");
            response.Headers.Append ("Content-Disposition",
                $"attachment; filename=\"{Path.GetFileName(filepath)}\"");
            
            await response.SendFileAsync(filepath);
        }

        public static async Task SendFileAssetAsync (this HttpResponse response, string filepath) 
        {
            string contentType = "unknown/unknown";
            Mime.TryGetContentType(filepath, out contentType);
            response.Headers.Append ("Content-Type", contentType);
            await response.SendFileAsync(filepath);
        }

        public static async Task NotFound (this HttpResponse response)
        {
            response.StatusCode = 404;
            await response.WriteAsync("Error 404: File not found. :(");
        }

        static FileExtensionContentTypeProvider Mime = new FileExtensionContentTypeProvider();
    }
}
