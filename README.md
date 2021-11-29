# PlayStation 3 game update download

Simple tool for downloading patch for games.

## build
For building app is require .net 6
> git clone https://github.com/3mam/ps3gut.git

#### build for Linux
> dotnet publish -r linux-x64 -c release

#### build for Windows 10
> dotnet publish -r win10-x64 -c release

copy **db.json** to that same place where is app


## example
> ./ps3gud -i BLES01807

This allowed to download all patch for GTA V