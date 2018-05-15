$ErrorActionPreference = "Stop"

$MainProj = "..\TweetDuck.csproj"
$BrowserProj = "..\subprocess\TweetDuck.Browser.csproj"

$Match = Select-String -Path $MainProj '<Import Project="packages\\cef\.redist\.x86\.(.*?)\\'
$Version = $Match.Matches[0].Groups[1].Value

Copy-Item "..\packages\cef.redist.x86.${Version}\CEF\devtools_resources.pak" -Destination "..\bld\Resources\" -Force

$Match = Select-String -Path $MainProj '<Import Project="packages\\CefSharp\.Common\.(.*?)\\'
$Version = $Match.Matches[0].Groups[1].Value

$Contents = [IO.File]::ReadAllText($BrowserProj)
$Contents = $Contents -Replace '(?<=<HintPath>\.\.\\packages\\CefSharp\.Common\.)(.*?)(?=\\)', $Version
$Contents = $Contents -Replace '(?<=<Reference Include="CefSharp\.BrowserSubprocess\.Core, Version=)(\d+)', $Version.Split(".")[0]

[IO.File]::WriteAllText($BrowserProj, $Contents)
