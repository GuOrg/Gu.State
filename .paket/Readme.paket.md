## General

1. PM> `.paket/paket.exe restore` restore packages.
2. PM> `.paket/paket.exe auto-restore on` restore packages on build.
3.a PM> `.paket/paket.exe update` update packages.
3.b PM> `.paket/paket.exe update group GROUPNAME` update packages.
4.a PM> `.paket/paket.exe install` install packages.
4.b PM> `.paket/paket.exe install --redirects` install packages and creates redirects.  
4.c PM> `.paket/paket.exe install -f --createnewbindingfiles` install packages and create app.configs with redirects.
5. PM> `.paket/paket.exe outdated` Lists all dependencies that have newer versions available.
6. PM> `.paket/paket.exe remove nuget Gu.Analyzers group Analyzers`

## Create packages

1. Build in release
2. PM> `.paket/paket.exe pack output publish symbols`
3. Packages are in the publish folder.

Docs: https://fsprojects.github.io/Paket/getting-started.html