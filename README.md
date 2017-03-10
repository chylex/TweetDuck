# Build Instructions

The program was build using Visual Studio 2013. After opening the solution, make sure you have **CefSharp.WinForms** and **Microsoft.VC120.CRT.JetBrains** included - if not, download them using NuGet.
```
PM> Install-Package CefSharp.WinForms -Version 55.0.0
PM> Install-Package Microsoft.VC120.CRT.JetBrains
```

After building, run **_postbuild.bat** which deletes unnecessary files that CefSharp adds after post-build events >_>

Built files are then available in **bin/x86** and/or **bin/x64**.
