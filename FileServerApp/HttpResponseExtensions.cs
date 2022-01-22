using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    }
}
