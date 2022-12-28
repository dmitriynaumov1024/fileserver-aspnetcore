using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class env 
{
    static IDictionary<string, string> readDotenvVariables ()
    {
        try {
            return File.ReadLines(".env")
                .Select(line => line.Split('='))
                .Where(words => words.Length == 2)
                .ToDictionary(kvpair => kvpair[0], kvpair => kvpair[1]);
        }
        catch (Exception) {
            return new Dictionary<string, string>();
        }
    }

    public static readonly IDictionary<string, string> dotenvVariables = readDotenvVariables();
    
    public static string get (string key) 
    {
        string result;
        dotenvVariables.TryGetValue(key, out result); 
        return result ?? Environment.GetEnvironmentVariable(key);
    }
}
