# File server

Yet another version of file server by me.

Old version with Vue 2 front-end is available on branch [v1](https://github.com/dmitriynaumov1024/fileserver-aspnetcore/tree/v1).

This variant of server is to work without any front-end framework and to be customizable (there shall be html building blocks, icons and a stylesheet in webapp/assets/).

Why this file server? Why once again? Because this one:

- has much more understandable request pipeline:

![pipeline.png](./science/pipeline.png)

- has nice front-end without any single line javascript:

![screenshot-mobile.png](./science/screenshot-mobile.png)

- looks neat on both mobile and desktop:

![screenshot-desktop.png](./science/screenshot-desktop.png)

- is easily customizable, even though it's pretty solid, you may want to adapt it for your needs.

## Prerequisites

- Any OS that supports .NET
- .NET SDK v6.0

## Usage

**Quick tryout**
```
cd webapp
dotnet run -- <root-directory> <port>
```

**Publish self-contained for Linux and run it**
```
cd webapp
dotnet publish --os linux --self-contained -p:PublishTrimmed=True -o ./dist

cd dist
./FileServer <root-directory> <port>
```

## Web outline

- `/fs/{path}` - shall return directory or file at given path relative to root-directory
- `/assets/{path}` - shall return asset file from webapp/assets/
- `/download/{path}` - shall return file at given path relative to root-directory as downloadable attachment 

## Copyright notes

- **.NET**: respecting original copyright, [more info here](https://dotnet.microsoft.com/)
- **File server**: MIT License  &copy; 2022  Dmitriy Naumov  naumov1024@gmail.com
