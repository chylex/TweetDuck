open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Threading.Tasks

// "$(DevEnvDir)CommonExtensions\Microsoft\FSharp\fsi.exe" "$(ProjectDir)Resources\PostBuild.fsx" --exec --nologo -- "$(TargetDir)\" "$(ProjectDir)\" "$(ConfigurationName)"
// "$(ProjectDir)bld\post_build.exe" "$(TargetDir)\" "$(ProjectDir)\" "$(ConfigurationName)"

exception ArgumentUsage of string

#if !INTERACTIVE
[<EntryPoint>]
#endif
let main (argv: string[]) =
    try
        if argv.Length < 2 then
            #if INTERACTIVE
            raise (ArgumentUsage "fsi.exe PostBuild.fsx --exec --nologo -- <TargetDir> <ProjectDir> [ConfigurationName] [VersionTag]")
            #else
            raise (ArgumentUsage "PostBuild.exe <TargetDir> <ProjectDir> [ConfigurationName] [VersionTag]")
            #endif
            
        let _time name func =
            let sw = Stopwatch.StartNew()
            func()
            sw.Stop()
            printfn "[%s took %i ms]" name (int (Math.Round(sw.Elapsed.TotalMilliseconds)))
            
        let (+/) path1 path2 =
            Path.Combine(path1, path2)
            
        let sw = Stopwatch.StartNew()
        
        // Setup
        
        let targetDir = argv.[0]
        let projectDir = argv.[1]
        
        let configuration = if argv.Length >= 3 then argv.[2]
                            else "Release"
                            
        let version = if argv.Length >= 4 then argv.[3]
                      else ((targetDir +/ "TweetDuck.exe") |> FileVersionInfo.GetVersionInfo).FileVersion
                      
        printfn "--------------------------"
        printfn "TweetDuck version %s" version
        printfn "--------------------------"
        
        let guideDir = targetDir +/ "guide"
        let localesDir = targetDir +/ "locales"
        let pluginsDir = targetDir +/ "plugins"
        let resourcesDir = targetDir +/ "resources"
        
        // Functions (File Management)
        
        let copyFile source target =
            File.Copy(source, target, true)
            
        let createDirectory path =
            Directory.CreateDirectory(path) |> ignore
            
        let rec copyDirectoryContentsFiltered source target (filter: string -> bool) =
            if not (Directory.Exists(target)) then
                Directory.CreateDirectory(target) |> ignore
                
            let src = DirectoryInfo(source)
            
            for file in src.EnumerateFiles() do
                if filter file.Name then
                    file.CopyTo(target +/ file.Name) |> ignore
                    
            for dir in src.EnumerateDirectories() do
                if filter dir.Name then
                    copyDirectoryContentsFiltered dir.FullName (target +/ dir.Name) filter
                    
        let copyDirectoryContents source target =
            copyDirectoryContentsFiltered source target (fun _ -> true)
            
        // Functions (File Processing)
        
        let byPattern path pattern =
            Directory.EnumerateFiles(path, pattern, SearchOption.AllDirectories)
            
        let exceptEndingWith (name: string) =
            Seq.filter (fun (file: string) -> not (file.EndsWith(name)))
            
        let iterateFiles (files: string seq) (func: string -> unit) =
            Parallel.ForEach(files, func) |> ignore
            
        let readFile file = seq {
            use stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan)
            use reader = new StreamReader(stream)
            let mutable cont = true
            
            while cont do
                let line = reader.ReadLine()
                
                if line = null then
                    cont <- false
                else
                    yield line
        }
        
        let writeFile (fullPath: string) (lines: string seq) =
            let relativePath = fullPath.[(targetDir.Length)..]
            File.WriteAllLines(fullPath, lines |> Seq.toArray)
            printfn "Processed %s" relativePath
        
        let processFiles (files: string seq) (extProcessors: IDictionary<string, (string seq -> string seq)>) =
            let rec processFileContents file =
                readFile file
                |> extProcessors.[Path.GetExtension(file)]
                
            iterateFiles files (fun file ->
                processFileContents file
                |> (writeFile file)
            )
            
        // Build
            
        copyFile (projectDir +/ "bld/Resources/LICENSES.txt") (targetDir +/ "LICENSES.txt")
        
        copyDirectoryContents (projectDir +/ "Resources/Guide") guideDir
        copyDirectoryContents (projectDir +/ "Resources/Content") resourcesDir
        
        createDirectory (pluginsDir +/ "official")
        createDirectory (pluginsDir +/ "user")
        
        copyDirectoryContentsFiltered
            (projectDir +/ "Resources/Plugins")
            (pluginsDir +/ "official")
            (fun name -> name <> ".debug" && name <> "emoji-instructions.txt")
            
        if configuration = "Debug" then
            copyDirectoryContents
                (projectDir +/ "Resources/Plugins/.debug")
                (pluginsDir +/ "user/.debug")
                
        if Directory.Exists(localesDir) || configuration = "Release" then
            Directory.EnumerateFiles(localesDir, "*.pak")
            |> exceptEndingWith @"\en-US.pak"
            |> Seq.iter File.Delete
            
        // Validation
        
        if File.ReadAllText(pluginsDir +/ "official/emoji-keyboard/emoji-ordering.txt").IndexOf('\r') <> -1 then
            raise (FormatException("emoji-ordering.txt must not have any carriage return characters"))
        else
            printfn "Verified emoji-ordering.txt"
            
        // Processing
        
        let fileProcessors =
            dict [
                ".js", id;
                ".css", id;
                ".html", id;
                ".meta", (fun (lines: string seq) ->
                    lines
                    |> Seq.map (fun line -> line.Replace("{version}", version))
                );
            ]
            
        processFiles (byPattern targetDir "*.js") fileProcessors
        processFiles (byPattern targetDir "*.css") fileProcessors
        processFiles (byPattern targetDir "*.html") fileProcessors
        processFiles (byPattern pluginsDir "*.meta") fileProcessors
        
        // Finished
        
        sw.Stop()
        printfn "------------------"
        printfn "Finished in %i ms" (int (Math.Round(sw.Elapsed.TotalMilliseconds)))
        printfn "------------------"
        0
        
    with
    | ArgumentUsage message ->
        printfn ""
        printfn "Build script usage:"
        printfn "%s" message
        printfn ""
        1
    | ex ->
        printfn ""
        printfn "Encountered an error while running PostBuild:"
        printfn "%A" ex
        printfn ""
        1
        
#if INTERACTIVE
printfn "Running PostBuild in interpreter..."
main (fsi.CommandLineArgs |> Array.skip (1 + (fsi.CommandLineArgs |> Array.findIndex (fun arg -> arg = "--"))))
#endif
