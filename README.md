# Support

[Follow TweetDuck on Twitter](https://twitter.com/TryMyAwesomeApp) &nbsp;|&nbsp; [Support via Ko-fi](https://ko-fi.com/chylex) &nbsp;|&nbsp; [Support via Patreon](https://www.patreon.com/chylex)

# Build Instructions

### Setup

The program can be built using Visual Studio 2019. Before opening the solution, please make sure you have the following workloads and components installed (optional components that are not listed can be deselected to save space):
* **.NET desktop development**
  * .NET Framework 4.7.2 SDK
  * F# desktop language support
* **Desktop development with C++**
  * MSVC v142 - VS 2019 C++ x64/x86 build tools (v14.20)

After opening the solution, right-click the solution and select **Restore NuGet Packages** to install all used libraries.

Development with Rider is also supported, as long as it is configured to use MSBuild from Visual Studio. You can set it in *File | Settings | Build, Execution, Deployment | Toolset and Build* with the `Use MSBuild version` drop-down.

### Debug

The `Debug` configuration uses a separate data folder by default (`%LOCALAPPDATA%\TweetDuckDebug`) to avoid affecting an existing installation of TweetDuck. You can modify this by opening **TweetDuck Properties** in Visual Studio, clicking the **Debug** tab, and changing the **Command line arguments** field.

While debugging, opening the main menu and clicking **Reload browser** automatically applies all changes to HTML/CSS/JS files in the `Resources` folder. This allows editing and testing resource files without restarting the program, but it will cause a short delay between browser reloads.

### Release

Open **Batch Build**, tick all `Release` configurations with `x86` platform, and click **Rebuild**. Check the status bar to make sure it says **Rebuild All succeeded**; if not, see the [Troubleshooting](#troubleshooting) section.

After the build succeeds, the `bin/x86/Release` folder will contain files intended for distribution (no debug symbols or other unnecessary files). You may package these files yourself, or see the [Installers](#installers) section for automated installer generation.

Building the `Release` configuration automatically generates the [update installer](#installers) if the environment is setup correctly. You can modify this behavior by opening `TweetDuck.csproj`, and editing the `<Target Name="FinalizeRelease" ...>` section.

If you decide to publicly release a custom version, please change all references to the TweetDuck name, website, and other links such as the issue tracker. The source files contain several constants and references to the official website and this repository, so don't forget to search all files for `chylex.com` and `github.com` in all files and replace them appropriately.

### Troubleshooting

#### Error: The command (...) exited with code 1
- This indicates a failed post-build event, open the **Output** tab for logs
- Determine if there was an IO error from the `rmdir` commands, the custom MSBuild targets near the end of the [.csproj file](https://github.com/chylex/TweetDuck/blob/master/TweetDuck.csproj), or in the **PostBuild.ps1** script (`Encountered an error while running PostBuild`)
- Some files are checked for invalid characters:
  - `Resources/Plugins/emoji-keyboard/emoji-ordering.txt` line endings must be LF (line feed); any CR (carriage return) in the file will cause a failed build, and you will need to ensure correct line endings in your text editor

### Installers

TweetDuck uses **Inno Setup** for installers and updates. First, download and install [InnoSetup 5.6.1](http://files.jrsoftware.org/is/5/innosetup-5.6.1.exe) (with Preprocessor support) and the [Inno Download Plugin 1.5.0](https://drive.google.com/folderview?id=0Bzw1xBVt0mokSXZrUEFIanV4azA&usp=sharing#list).

Next, add the Inno Setup installation folder (usually `C:\Program Files (x86)\Inno Setup 5`) into your **PATH** environment variable. You may need to restart File Explorer and Visual Studio for the change to take place.

Now you can generate installers by running `bld/GEN INSTALLERS.bat`. Note that this will only package the files, you still need to run the [release build](#release) in Visual Studio first!

After the window closes, three installers will be generated inside the `bld/Output` folder:
* **TweetDuck.exe**
  * This is the main installer that creates entries in the Start Menu & Programs and Features, and an optional desktop icon
* **TweetDuck.Update.exe**
  * This is a lightweight update installer that only contains the most important files that usually change across releases
  * It will automatically download and apply the full installer if the user's current version of CEF does not match
* **TweetDuck.Portable.exe**
  * This is a portable installer that does not need administrator privileges
  * It automatically creates a `makeportable` file in the program folder, which forces TweetDuck to run in portable mode

If you plan to distribute your own installers, you can change the variables in the installer files (`.iss`) and in the update system to point to your own repository, and use the power of the existing update system.

#### Notes

> There is a small chance running `GEN INSTALLERS.bat` immediately shows a resource error. If that happens, close the console window (which terminates all Inno Setup processes and leaves corrupted installers in the output folder), and run it again.

> Running `GEN INSTALLERS.bat` uses about 400 MB of RAM due to high compression. You can lower this to about 140 MB by opening `gen_full.iss` and `gen_port.iss`, and changing `LZMADictionarySize=15360` to `LZMADictionarySize=4096`.
