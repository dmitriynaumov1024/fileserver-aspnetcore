using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace FileServerApp
{
    public static class FsExtensions
    {
        public static Dictionary<string, object> GetFsEntries (string relPath, string rootPath) {
            string path = rootPath + relPath;
            IEnumerable<string> subdirs = null, files = null;
            var baseDir = new DirectoryInfo(path);
            var rootDir = new DirectoryInfo(rootPath);
            string parent = "";

            if (baseDir.Exists) {
                // Get subdirs and files
                subdirs = baseDir.GetDirectories().Select(item => item.Name);
                files = baseDir.GetFiles().Select(item => item.Name);
                parent = GetRelativePath(baseDir.Parent, rootDir);
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

        public static string GetRelativePath (DirectoryInfo directory, DirectoryInfo root)
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
    }
}