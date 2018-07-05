# Support

[Follow TweetDuck on Twitter](https://twitter.com/TryMyAwesomeApp) &nbsp;|&nbsp; [Support via PayPal](https://paypal.me/chylex) &nbsp;|&nbsp; [Support via Patreon](https://www.patreon.com/chylex)

# Build Instructions

### Setup

The program was built using Visual Studio 2017. Before opening the solution, please make sure you have the following workloads and components installed (optional components that are not listed can be deselected to save space):
* **.NET desktop development**
  * .NET Framework 4 – 4.6 development tools
  * F# desktop language support
* **Desktop development with C++**
  * VC++ 2017 latest v141 tools

After opening the solution, right-click the solution and select **Restore NuGet Packages**, or manually run this command in the **Package Manager Console**:
```
PM> Install-Package CefSharp.WinForms -Version 67.0.0-CI2658 -Source https://www.myget.org/F/cefsharp/api/v3/index.json
```

Note that some pre-release builds of CefSharp are not available on NuGet. To correctly restore packages in that case, open **Package Manager Settings**, and add `https://www.myget.org/F/cefsharp/api/v3/index.json` to the list of package sources.

### Debug

The `Debug` configuration uses a separate data folder by default (`%LOCALAPPDATA%\TweetDuckDebug`) to avoid affecting an existing installation of TweetDuck. You can modify this by opening **TweetDuck Properties** in Visual Studio, clicking the **Debug** tab, and changing the **Command line arguments** field.

While debugging, opening the main menu and clicking **Reload browser** automatically rebuilds all resources in the `Resources/Scripts` and `Resources/Plugins` folders. This allows editing HTML/CSS/JS files and applying the changes without restarting the program, but it will cause a short delay between browser reloads.

### Release

Open **Batch Build**, tick all `Release` configurations with `x86` platform, and click **Rebuild**. Check the status bar to make sure it says **Rebuild All succeeded**; if not, see the [Troubleshooting](#troubleshooting) section.

After the build succeeds, the `bin/x86/Release` folder will contain files intended for distribution (no debug symbols or other unnecessary files). You may package these files yourself, or see the [Installers](#installers) section for automated installer generation.

The `Release` configuration omits debug symbols and other unnecessary files. You can modify this behavior by opening `TweetDuck.csproj`, and editing the `<Target Name="AfterBuild" Condition="$(ConfigurationName) == Release">` section.

If you decide to publicly release a custom version, please make it clear that it is not an official release of TweetDuck. There are many references to the official website and this repository, especially in the update system, so search for `chylex.com` and `github.com` in all files and replace them appropriately.

### Troubleshooting

#### Error: The command (...) exited with code 1
- This indicates a failed post-build event, open the **Output** tab for logs
- Determine if there was an IO error from the `rmdir` commands, or whether the error was in the **PostBuild.ps1** script (`Encountered an error while running PostBuild.ps1 on line <xyz>`)
- Some files are checked for invalid characters:
  - `Resources/Plugins/emoji-keyboard/emoji-ordering.txt` line endings must be LF (line feed); any CR (carriage return) in the file will cause a failed build, and you will need to ensure correct line endings in your text editor

### Installers

TweetDuck uses **Inno Setup** for installers and updates. First, download and install [InnoSetup QuickStart Pack](http://www.jrsoftware.org/isdl.php) (non-unicode; editor and encryption support not required) and the [Inno Download Plugin](https://code.google.com/archive/p/inno-download-plugin).

Next, add the Inno Setup installation folder (usually `C:\Program Files (x86)\Inno Setup 5`) into your **PATH** environment variable. You may need to restart File Explorer for the change to take place.

Now you can generate installers after a build by running `bld/RUN BUILD.bat`. Note that this will only package the files, you still need to run the [release build](#release) in Visual Studio!

After the window closes, three installers will be generated inside the `bld/Output` folder:
* **TweetDuck.exe**
  * This is the main installer that creates entries in the Start Menu & Programs and Features, and an optional desktop icon
* **TweetDuck.Update.exe**
  * This is a lightweight update installer that only contains the most important files that usually change across releases
  * It will automatically download and apply the full installer if the user's current version of CEF does not match (the download link is in `gen_upd.iss` and points to this repository by default)
* **TweetDuck.Portable.exe**
  * This is a portable installer that does not need administrator privileges
  * It automatically creates a `makeportable` file in the program folder, which forces TweetDuck to run in portable mode

#### Notes

> When opening **Batch Build**, you will also see `x64` and `AnyCPU` configurations. These are visible due to what I consider a Visual Studio bug, and will not work without significant changes to the project. Manually running the `Resources/PostCefUpdate.ps1` PowerShell script modifies the downloaded CefSharp packages, and removes the invalid configurations.

> There is a small chance running `RUN BUILD.bat` immediately shows a resource error. If that happens, close the console window (which terminates all Inno Setup processes and leaves corrupted installers in the output folder), and run it again.

> Running `RUN BUILD.bat` uses about 400 MB of RAM due to high compression. You can lower this to about 140 MB by opening `gen_full.iss` and `gen_port.iss`, and changing `LZMADictionarySize=15360` to `LZMADictionarySize=4096`.
