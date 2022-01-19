# File server

## Summary

This is a simple file server app built with [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) framework. You can easily browse directories and view files with this app. To get started,
1. Download [.NET sdk](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) (this project requires .NET sdk 5.0)
2. Build the project: go to FileServerApp and `dotnet build`
3. Go to build output (something like bin/Debug/netcoreapp3.1/)
4. Put your files/folders to wwwroot directory in build output
5. Launch the executable file (FileServerApp)
6. Type `http://localhost:5000/` in your browser
7. Enjoy!

## What's inside

This application's backend is written in C#. Frontend is written using [Vue.js](https://vuejs.org) framework.

File Program.cs is used to initialize the web host builder and build the application.
File Startup.cs is used to configure the application on startup.
File Handlers.cs contains 2 request delegates:
- DefaultGet - default fallback delegate
- FileServer - get a file or directory info depending on request path.
File index.html is the only frontend file. It contains all necessary markup, styles and script. 