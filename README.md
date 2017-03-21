# Build Instructions

The program was build using Visual Studio 2013. After opening the solution, make sure you have **CefSharp.WinForms** and **Microsoft.VC120.CRT.JetBrains** included - if not, download them using NuGet.
```
PM> Install-Package CefSharp.WinForms -Version 55.0.0
PM> Install-Package Microsoft.VC120.CRT.JetBrains
```

After building, run either `_postbuild.bat` if you want to package the files yourself, or `bld/RUN BUILD.bat` to generate installer files using Inno Setup (make sure the Inno Setup binaries are on your PATH).

Built files are then available in **bin/x86**, installer files are generated in **bld/Output**. If you decide to release a custom version publicly, please make it clear that it is not the original TweetDuck. 
