# Build Instructions

### Setup

The program was built using Visual Studio 2017. Before opening the solution, please make sure you have the following workloads and components installed (optional components that are not listed can be deselected to save space):
* **.NET desktop development**
  * .NET Framework 4 – 4.6 development tools
* **Desktop development with C++**
  * VC++ 2017 v141 toolset

After opening the solution, download the following NuGet packages by right-clicking on the solution and selecting **Restore NuGet Packages**, or manually running these commands in the **Package Manager Console**:
```
PM> Install-Package CefSharp.WinForms -Version 57.0.0
PM> Install-Package Microsoft.VC120.CRT.JetBrains
```

### Debug

It is recommended to create a separate data folder for debugging, otherwise you will not be able to run TweetDuck while debugging the solution. To do that, open **TweetDuck Properties**, click the **Debug** tab, make sure your **Configuration** is set to `Active (Debug)` (or just `Debug`), and insert this into the **Command line arguments** field:
```
-datafolder TweetDuckDebug
```

### Build

To make a release build of TweetDuck, open **Batch Build**, tick all `Release` configurations except for the `UnitTest` project (otherwise the build will fail), and click **Rebuild**. Check the status bar to make sure it says **Rebuild All succeeded**; if not, open the **Output** view and see which part of the build failed.

After the build succeeds, the **bin/x86/Release** folder will contain files intended for distribution (no debug symbols or other unnecessary files). You may package these files yourself, or see the [Installers](#Installers) section for automated installer generation.

If you decide to release a custom version publicly, please make it clear that it is not an official release of TweetDuck.

### Installers

TweetDuck uses **Inno Setup** to automate the creation of installers. First, download and install [InnoSetup QuickStart Pack](http://www.jrsoftware.org/isdl.php) (non-unicode; editor and encryption support not required) and the [Inno Download Plugin](https://code.google.com/archive/p/inno-download-plugin).

Next, add the Inno Setup installation folder (usually `C:\Program Files (x86)\Inno Setup 5`) into your **PATH** environment variable. You may need to restart File Explorer for the change to take place.

Now you can generate installers after a build by running **bld/RUN BUILD.bat**. Note that despite the name, this will only package the files, you still need to run the [build](#Build) in Visual Studio!

After the window closes, three installers will be generated inside the **bld/Output** folder:
* **TweetDuck.exe**
  * This is the main installer that creates entries in the Start Menu & Programs and Features, and an optional desktop icon
* **TweetDuck.Update.exe**
  * This is a lightweight update installer that only contains the most important files that usually change across releases
  * It will automatically download and apply the full installer if the user's current version of CEF does not match (the download link is in `gen_upd.iss` and points to this repository by default)
* **TweetDuck.Portable.exe**
  * This is a portable installer that can be ran without administrator privileges
  * It automatically creates a `makeportable` file in the program folder, which forces TweetDuck to run in portable mode

Note: There is a small chance you will see a resource error when running `RUN BUILD.bat`. If that happens, close the console window (which will terminate all Inno Setup processes and leave corrupted installer files in the output folder), and run it again.

### Code Notes

There are many references to the official TweetDuck website and this repository in the code and installers, so if you plan to release your own version, make sure to search for `tweetduck.chylex.com` and `github.com` in the whole repository and replace them appropriately.
