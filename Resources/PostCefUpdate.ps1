$ErrorActionPreference = "Stop"

try{
  $mainProj = "..\TweetDuck.csproj"
  $browserProj = "..\subprocess\TweetDuck.Browser.csproj"
  
  $cefMatch = Select-String -Path $mainProj '<Import Project="packages\\cef\.redist\.x86\.(.*?)\\'
  $cefVersion = $cefMatch.Matches[0].Groups[1].Value
  
  $sharpMatch = Select-String -Path $mainProj '<Import Project="packages\\CefSharp\.Common\.(.*?)\\'
  $sharpVersion = $sharpMatch.Matches[0].Groups[1].Value
  
  $propsFiles = "..\packages\CefSharp.Common.${sharpVersion}\build\CefSharp.Common.props",
                "..\packages\CefSharp.WinForms.${sharpVersion}\build\CefSharp.WinForms.props"
  
  $targetFiles = "..\packages\CefSharp.Common.${sharpVersion}\build\CefSharp.Common.targets"
  
  # Greetings
  
  $title = "CEF ${cefVersion}, CefSharp ${sharpVersion}"
  
  Write-Host ("-" * $title.Length)
  Write-Host $title
  Write-Host ("-" * $title.Length)
  
  # Perform update
  
  Write-Host "Copying dev tools to repository..."
  
  Copy-Item "..\packages\cef.redist.x86.${cefVersion}\CEF\devtools_resources.pak" -Destination "..\bld\Resources\" -Force
  
  Write-Host "Updating browser subprocess reference..."
  
  $contents = [IO.File]::ReadAllText($browserProj)
  $contents = $contents -Replace '(?<=<HintPath>\.\.\\packages\\CefSharp\.Common\.)(.*?)(?=\\)', $sharpVersion
  $contents = $contents -Replace '(?<=<Reference Include="CefSharp\.BrowserSubprocess\.Core, Version=)(\d+)', $sharpVersion.Split(".")[0]
  
  [IO.File]::WriteAllText($browserProj, $contents)
  
  Write-Host "Removing x64 and AnyCPU from package files..."
  
  foreach($file in $propsFiles){
    $contents = [IO.File]::ReadAllText($file)
    $contents = $contents -Replace '(?<=<When Condition=")(''\$\(Platform\)'' == ''(AnyCPU|x64)'')(?=">)', 'false'
    
    [IO.File]::WriteAllText($file, $contents)
  }
  
  foreach($file in $targetFiles){
    $contents = [IO.File]::ReadAllText($file)
    $contents = $contents -Replace '(?<=<ItemGroup Condition=")(''\$\(Platform\)'' == ''(AnyCPU|x64)'')(?=">)', 'false'
    
    [IO.File]::WriteAllText($file, $contents)
  }
  
  # Finished
  
  Write-Host ""
  Write-Host "Finished. Exiting in 6 seconds..."
  Start-Sleep -Seconds 6
  
}catch{
  Write-Host ""
  Write-Host "Encountered an error while running PostBuild.ps1 on line" $_.InvocationInfo.ScriptLineNumber
  Write-Host $_
  
  $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
  Exit 1
}
