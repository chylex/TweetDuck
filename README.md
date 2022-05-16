[Follow TweetDuck on Twitter](https://twitter.com/TryMyAwesomeApp) &nbsp;|&nbsp; [Support via Ko-fi](https://ko-fi.com/chylex) &nbsp;|&nbsp; [Support via Patreon](https://www.patreon.com/chylex)

# Table of Contents

1. [Installation](#installation)
2. [Source Code](#source-code)
    * [Requirements](#requirements)
        + [Editors](#editors)
        + [Installers](#installers)
    * [Solution Overview](#solution-overview)
        + [Core Libraries](#core-libraries)
            - [TweetLib.Core](#tweetlibcore)
            - [TweetLib.Browser](#tweetlibbrowser)
            - [TweetLib.Browser.CEF](#tweetlibbrowsercef)
        + [Windows Projects](#windows-projects)
            - [TweetDuck](#tweetduck)
            - [TweetDuck.Browser](#tweetduckbrowser)
            - [TweetDuck.Video](#tweetduckvideo)
            - [TweetImpl.CefSharp](#tweetimplcefsharp)
        + [Linux Projects](#linux-projects)
            - [TweetDuck](#tweetduck-1)
            - [TweetImpl.CefGlue](#tweetimplcefglue)
        + [Miscellaneous](#miscellaneous)
            - [TweetLib.Communication](#tweetlibcommunication)
            - [TweetLib.Utils](#tweetlibutils)
            - [TweetTest.*](#tweettest)
3. [Development (Windows)](#development-windows)
    * [Building](#building)
    * [Debugging](#debugging)
    * [Release](#release)
        + [Installers](#installers-1)
4. [Development (Linux)](#development-linux)
    * [Building](#building-1)
    * [Release](#release-1)

# Installation

Download links and system requirements are on the [official website](https://tweetduck.chylex.com).

# Source Code

## Requirements

Building TweetDuck for Windows requires at minimum [Visual Studio 2019](https://visualstudio.microsoft.com/downloads) and Windows 7. Before opening the solution, open Visual Studio Installer and make sure you have the following Visual Studio workloads and components installed:
* **.NET desktop development**
  * .NET SDK
  * F# desktop language support
* **Desktop development with C++**
  * MSVC v142 - VS 2019 C++ x64/x86 build tools (v14.20 / Latest)

In the **Installation details** panel, you can expand the workloads you selected, and uncheck any components that are not listed above to save space. You may uncheck the .NET SDK component if you installed the [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) directly.

Building TweetDuck for Linux requires [.NET 6 SDK](https://docs.microsoft.com/en-us/dotnet/core/install/linux). The Linux project has its own solution file in the `linux/` folder.

### Editors

For editing code, I recommend either:

* [Visual Studio](https://visualstudio.microsoft.com/downloads/) for C# / F# + [VS Code](https://code.visualstudio.com/) for the rest (free when using the Community edition of Visual Studio)
* [Rider](https://www.jetbrains.com/rider/) for all languages (paid)

Icons and logos were designed in [Affinity Designer](https://affinity.serif.com/en-us/designer/) (paid). The original design projects are in the `resources/Design/` folder (`.afdesign` extension).

### Installers

> If you don't want to build installers using the existing foundations, you can skip this section.

Official Windows installers are built using [InnoSetup](https://jrsoftware.org/isinfo.php) and [Inno Download Plugin](https://mitrichsoftware.wordpress.com/inno-setup-tools/inno-download-plugin/), specifically:
* [InnoSetup 5.6.1](https://files.jrsoftware.org/is/5/innosetup-5.6.1.exe) with Preprocessor support
* [Inno Download Plugin 1.5.0](https://drive.google.com/folderview?id=0Bzw1xBVt0mokSXZrUEFIanV4azA&usp=sharing#list)

When installing InnoSetup, you can choose to include Inno Script Studio which I recommend for editing and testing installer configuration files in the `bld` folder (`.iss` extension).

Scripts for building installers require the `PATH` environment variable to include the InnoSetup installation folder. You can either edit `PATH` manually, or use a program like [Rapid Environment Editor](https://www.rapidee.com/en/about) to simplify the process. For example, this is the installation folder I added to `PATH` under **User variables**:
* `C:\Program Files (x86)\Inno Setup 5`

You may need to restart Visual Studio after changing `PATH` for the change to take place.

## Solution Overview

Open the solution file `TweetDuck.sln` (or `linux/TweetDuck.Linux.sln`) in an IDE, and use the **Restore NuGet Packages** option in your IDE to install dependencies.

On Windows, TweetDuck uses the [CefSharp](https://github.com/cefsharp/CefSharp/) library for the browser component, and Windows Forms for the GUI.

On Linux, TweetDuck uses the [ChromiumGtk](https://github.com/lunixo/ChromiumGtk) library, which combines [CefGlue](https://gitlab.com/xiliumhq/chromiumembedded/cefglue) for the browser component and [GtkSharp](https://github.com/GtkSharp/GtkSharp) for the GUI.

The solution contains several C# projects for executables and libraries, and F# projects for automated tests. All projects target `.NET 6`.

Projects are organized into folders:
* Windows projects are in the `windows/` folder
* Linux projects are in the `linux/` folder
* Libraries (`TweetLib.*`) are in the `lib/` folder
* Tests (`TweetTest.*`) are also in the `lib/` folder

Here are a few things to keep in mind:
* Executable projects have their entry points in `Program.cs`
* Library projects targeting `.NET Standard` have their assembly information in `Lib.cs`
* All non-test projects include a link to the `Version.cs` file in the root of the repository, which allows changing the version of all executables and library files in one place

Web resource files (HTML, CSS, JS) are in the `Resources/` folder:
* `Resources/Content/` contains all the core features of TweetDuck injected into the browser components
* `Resources/Guide/` contains the official TweetDuck guide that opens as a popup
* `Resources/Plugins/` contains all official plugins, and a `.debug` plugin for testing

These resource folders are linked as part of the `TweetLib.Core` project so they can be edited directly within an IDE. Alternatively, you can edit them using [VS Code](https://code.visualstudio.com/) by opening the workspace file `Resources/..code-workspace`.

### Core Libraries

#### TweetLib.Core

This library contains the core TweetDuck application and browser logic. It is built around simple dependency injection that makes it independent of any concrete OS, GUI framework, or browser implementation.

To simplify porting to other systems, it is not necessary to implement all interfaces, but some functionality will be missing (for ex. if clipboard-related interfaces are not implemented, then context menus will not contain options to copy text or images to clipboard).

#### TweetLib.Browser

This library provides a zero-dependency abstraction of browser components and logic. It defines interfaces, events, and container objects that are used by the `TweetLib.Core` library to describe how a browser should behave, while making as few assumptions about the actual browser implementation as possible.

#### TweetLib.Browser.CEF

This library is a partial implementation of `TweetLib.Browser` based on [CEF](https://bitbucket.org/chromiumembedded/cef/) interfaces and conventions.

While `TweetLib.Browser` is highly generic, most browser libraries are likely to be using some form of [CEF](https://bitbucket.org/chromiumembedded/cef/), so this library significantly reduces the amount of work required to swap between browser libraries that are based on [CEF](https://bitbucket.org/chromiumembedded/cef/).

### Windows Projects

#### TweetDuck

Main Windows executable. It has a dependency on [CefSharp](https://github.com/cefsharp/CefSharp/) and Windows Forms. Here you will find the entry point that bootstraps the main application, as well as code for GUIs and Windows-specific functionality.

#### TweetDuck.Browser

Windows executable that hosts various Chromium processes. It has a dependency on [CefSharp](https://github.com/cefsharp/CefSharp/).

#### TweetDuck.Video

Windows executable that hosts a video player, which is based on the WMPLib ActiveX component responsible for integrating Windows Media Player into .NET Framework.

By default, [CefSharp](https://github.com/cefsharp/CefSharp/) is not built with support for H.264 video playback due to software patent nonsense, and even though TweetDuck could be moved entirely to Europe where MPEG LA's patent means nothing, it would require building a custom version of Chromium which requires too many resources. Instead, when a Twitter video played, TweetDuck launches this video player process, which uses Windows Media Player to play H.264 videos.

#### TweetImpl.CefSharp

Windows library that implements `TweetLib.Browser.CEF` using the [CefSharp](https://github.com/cefsharp/CefSharp/) library and Windows Forms.

### Linux Projects

#### TweetDuck

Main Linux executable. It has a transitive dependency on [ChromiumGtk](https://github.com/lunixo/ChromiumGtk). Here you will find the entry point that bootstraps the main application, as well as code for GUIs and Linux-specific functionality.

#### TweetImpl.CefGlue

Linux library that implements `TweetLib.Browser.CEF` using [ChromiumGtk](https://github.com/lunixo/ChromiumGtk), which is based on [CefGlue](https://gitlab.com/xiliumhq/chromiumembedded/cefglue) and [GtkSharp](https://github.com/GtkSharp/GtkSharp).

### Miscellaneous

#### TweetLib.Communication

This library provides a `DuplexPipe` class for two-way communication between processes.

#### TweetLib.Utils

This library contains various utilities that fill some very specific holes in the .NET standard library.

#### TweetTest.*

These are F# projects with automated tests.

# Development (Windows)

When developing with [Rider](https://www.jetbrains.com/rider/), it must be configured to use MSBuild from Visual Studio, and the `DevEnvDir` property must be set to the full path to the `Common7\IDE` folder which is inside Visual Studio's installation folder. You can set both in **File | Settings | Build, Execution, Deployment | Toolset and Build**:

1. Click the `MSBuild version` drop-down, and select the path that includes the Visual Studio installation folder.
2. Click the Edit button next to `MSBuild global properties`.
3. Add a new property named `DevEnvDir`, and set its value to the full path to `Common7\IDE`. For example:
   - `VS 2019 Community` - `C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE`
   - `VS 2022 Community` - `C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE`

## Building

The `windows/TweetDuck/TweetDuck.csproj` project file has several tasks (targets) that run before and after a build:
* `PreBuildEvent` runs a PowerShell script that kills `TweetDuck.Browser` processes, in case they got stuck
* `CopyResources` copies resource files into the build folder, and patches and validates them using the `PostBuild.ps1` PowerShell script
* `FinalizeDebug` copies a debug plugin (`Resources/Plugins/.debug`) into the build folder (Debug only)
* `FinalizeRelease` prepares the build folder for publishing, and if InnoSetup is installed, regenerates the [update installer](#installers-1) (Release only)

If the build fails, usually with an error like `The command (...) exited with code 1`, open the **Output** tab for detailed logs. A possible cause is the `PostBuild.ps1` script's file validation:
* `Resources/Plugins/emoji-keyboard/emoji-ordering.txt` line endings must be LF (line feed); if the file contains any CR (carriage return) characters, the build will fail

## Debugging

The `Debug` configuration uses a separate data folder by default (`%LOCALAPPDATA%\TweetDuckDebug`) to avoid affecting an existing installation of TweetDuck. You can modify this by opening **TweetDuck Properties** in Visual Studio, clicking the **Debug** tab, and changing the **Command line arguments** field.

While debugging, opening the main menu and clicking **Reload browser** automatically applies all changes to HTML/CSS/JS files in the `Resources/` folder. This allows editing and testing resource files without restarting the program, but it will cause a short delay between browser reloads.

## Release

Open **Batch Build**, tick all `Release` configurations with `x86` platform, and click **Rebuild**. Check the status bar to make sure it says **Rebuild All succeeded**; if not, see the end of the [Building](#building) section.

If the build succeeds, the `windows/TweetDuck/bin/x86/Release` folder will contain files intended for distribution (no debug symbols or other unnecessary files). You may package these files yourself, or see the [Installers](#installers-1) section for automated installer generation.

If you decide to publicly release a custom version, please change all references to the TweetDuck name, website, and other links such as the issue tracker. The source files contain several constants and references to the official website and this repository, so don't forget to search all files for `chylex.com` and `github.com` in all files and replace them appropriately.

### Installers

If you have all the requirements for building [installers](#installers), you can generate them by running `bld/GEN INSTALLERS.bat`. Note that this will only package the files, you still need to create a [release build](#release) in Visual Studio first!

After the window closes, three installers will be generated inside the `bld/Output/` folder:
* **TweetDuck.exe**
  * This is the main installer that creates entries in the Start Menu & Programs and Features, and an optional desktop icon
* **TweetDuck.Update.exe**
  * This is a lightweight update installer that only contains the most important files that usually change across releases
  * It will automatically download and apply the full installer if the user's current version of CEF does not match
* **TweetDuck.Portable.exe**
  * This is a portable installer that does not need administrator privileges
  * It automatically creates a `makeportable` file in the program folder, which forces TweetDuck to run in portable mode

If you plan to distribute your own installers, you can change the variables in the `.iss` installer files and in the update system to point to your own repository, and use the power of the existing update system.

> There is a small chance running `GEN INSTALLERS.bat` immediately shows a resource error. If that happens, close the console window (which terminates all Inno Setup processes and leaves corrupted installers in the output folder), and run it again.

> Running `GEN INSTALLERS.bat` uses about 400 MB of RAM due to high compression. You can lower this to about 140 MB by opening `gen_full.iss` and `gen_port.iss`, and changing `LZMADictionarySize=15360` to `LZMADictionarySize=4096`.

# Development (Linux)

Unfortunately the development experience on Linux is terrible, likely due to mixed C# and native code. The .NET debugger seems to crash the moment it enters native code, so the only way to run the app is without the debugger attached. If any C# code throws an exception, it will crash the whole application with no usable stack trace or error message. Please let me know if you find a way to make this better.

## Building

The `linux/TweetDuck/TweetDuck.csproj` project file has several tasks (targets) that run after a build:

* `CopyResources` copies resource files into the build folder, and patches and validates them using the `build.sh` Bash script
* `FinalizeDebug` copies a debug plugin (`Resources/Plugins/.debug`) into the build folder (Debug only)
* `FinalizeRelease` prepares the build folder for publishing (Release only)

## Release

To change the application version before a release, search for the `<Version>` tag in every `.csproj` file in the `linux/` folder and modify it.

To build the application, execute the `linux/publish.sh` Bash script. This will build the Release configuration for the `linux-x64` runtime platform, and create a tarball in the `linux/bld/` folder.

If you decide to publicly release a custom version, please change all references to the TweetDuck name, website, and other links such as the issue tracker. The source files contain several constants and references to the official website and this repository, so don't forget to search all files for `chylex.com` and `github.com` in all files and replace them appropriately.
