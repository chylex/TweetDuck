# Build Instructions

The program was build using Visual Studio 2013. After opening the solution, make sure you have **CefSharp.WinForms** and **Microsoft.VC120.CRT.JetBrains** included - if not, download them using NuGet. For **CefSharp**, you will need version 49 or newer currently available as a pre-release.
```
PM> Install-Package CefSharp.WinForms -Pre
PM> Install-Package Microsoft.VC120.CRT.JetBrains
```

TweetD\*ck comes in two variants - TweetDick and TweetDuck. The solution includes both configurations under the names **Release Dick** and **Release Duck**, so make sure to select the correct one, or build both using Batch Build.

After building, run **_postbuild.bat** which deletes unnecessary files that CefSharp adds after post-build events >_>

Built files are then available in **bin/x86** and/or **bin/x64**.
