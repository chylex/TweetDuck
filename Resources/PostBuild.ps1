Param(
  [Parameter(Mandatory = $True, Position = 1)][string] $targetDir,
  [Parameter(Mandatory = $True, Position = 2)][string] $projectDir,
  [Parameter(Position = 3)][string] $configuration = "Release",
  [Parameter(Position = 4)][string] $version = ""
)

$ErrorActionPreference = "Stop"

try{
  $sw = [Diagnostics.Stopwatch]::StartNew()
  
  if ($version.Equals("")){
    $version = (Get-Item (Join-Path $targetDir "TweetDuck.exe")).VersionInfo.FileVersion
  }
  
  Write-Host "--------------------------"
  Write-Host "TweetDuck version" $version
  Write-Host "--------------------------"
  
  # Cleanup
  
  if (Test-Path (Join-Path $targetDir "locales")){
    Remove-Item -Path (Join-Path $targetDir "locales\*.pak") -Exclude "en-US.pak"
  }
  
  # Copy resources
  
  Copy-Item (Join-Path $projectDir "bld\Resources\LICENSES.txt") -Destination $targetDir -Force
  
  New-Item -ItemType directory -Path $targetDir -Name "scripts" | Out-Null
  New-Item -ItemType directory -Path $targetDir -Name "plugins" | Out-Null
  New-Item -ItemType directory -Path $targetDir -Name "plugins\official" | Out-Null
  New-Item -ItemType directory -Path $targetDir -Name "plugins\user" | Out-Null
  
  Copy-Item (Join-Path $projectDir "Resources\Scripts\*") -Recurse -Destination (Join-Path $targetDir "scripts")
  Copy-Item (Join-Path $projectDir "Resources\Plugins\*") -Recurse -Destination (Join-Path $targetDir "plugins\official") -Exclude ".debug"
  
  Remove-Item (Join-Path $targetDir "plugins\official\emoji-keyboard\emoji-instructions.txt")
  
  if ($configuration -eq "Debug"){
    New-Item -ItemType directory -Path $targetDir -Name "plugins\user\.debug" | Out-Null
    Copy-Item (Join-Path $projectDir "Resources\Plugins\.debug\*") -Recurse -Destination (Join-Path $targetDir "plugins\user\.debug")
  }
  
  # Helper functions
  
  function Remove-Empty-Lines{
    Param([Parameter(Mandatory = $True, Position = 1)] $lines)
    
    foreach($line in $lines){
      if ($line -ne ''){
        $line
      }
    }
  }
  
  function Check-Carriage-Return{
    Param([Parameter(Mandatory = $True, Position = 1)] $file)
    
    if (!(Test-Path $file)){
      Throw "$file does not exist"
    }
    
    if ((Get-Content -Path $file -Raw).Contains("`r")){
      Throw "$file must not have any carriage return characters"
    }
    
    Write-Host "Verified" $file.Substring($targetDir.Length)
  }
  
  function Rewrite-File{
    Param([Parameter(Mandatory = $True, Position = 1)] $file,
          [Parameter(Mandatory = $True, Position = 2)] $lines,
          [Parameter(Mandatory = $True, Position = 3)] $imports)
    
    $lines = Remove-Empty-Lines($lines)
    $relativePath = $file.FullName.Substring($targetDir.Length)
    
    foreach($line in $lines){
      if ($line.Contains('#import ')){
        $imports.Add($file.FullName)
        break
      }
    }
    
    if ($relativePath.StartsWith("scripts\")){
      $lines = (,("#" + $version) + $lines)
    }
    
    [IO.File]::WriteAllLines($file.FullName, $lines)
    Write-Host "Processed" $relativePath
  }
  
  # Validation
  
  Check-Carriage-Return(Join-Path $targetDir "plugins\official\emoji-keyboard\emoji-ordering.txt")
  
  # Processing
  
  $imports = New-Object "System.Collections.Generic.List[string]"
  
  foreach($file in Get-ChildItem -Path $targetDir -Filter "*.js" -Exclude "configuration.default.js" -Recurse){
    $lines = [IO.File]::ReadLines($file.FullName)
    $lines = $lines | ForEach-Object { $_.TrimStart() }
    $lines = $lines -Replace '^(.*?)((?<=^|[;{}()])\s?//(?:\s.*|$))?$', '$1'
    $lines = $lines -Replace '(?<!\w)(return|throw)(\s.*?)? if (.*?);', 'if ($3)$1$2;'
    Rewrite-File $file $lines $imports
  }
  
  foreach($file in Get-ChildItem -Path $targetDir -Filter "*.css" -Recurse){
    $lines = [IO.File]::ReadLines($file.FullName)
    $lines = $lines -Replace '\s*/\*.*?\*/', ''
    $lines = $lines -Replace '^(\S.*) {$', '$1{'
    $lines = $lines -Replace '^\s+(.+?):\s*(.+?)(?:\s*(!important))?;$', '$1:$2$3;'
    $lines = @((Remove-Empty-Lines($lines)) -Join ' ')
    $lines = $lines -Replace '([{};])\s', '$1'
    $lines = $lines -Replace ';}', '}'
    Rewrite-File $file $lines $imports
  }
  
  foreach($file in Get-ChildItem -Path $targetDir -Filter "*.html" -Recurse){
    $lines = [IO.File]::ReadLines($file.FullName)
    $lines = $lines | ForEach-Object { $_.TrimStart() }
    Rewrite-File $file $lines $imports
  }
  
  foreach($file in Get-ChildItem -Path (Join-Path $targetDir "plugins") -Filter "*.meta" -Recurse){
    $lines = [IO.File]::ReadLines($file.FullName)
    $lines = $lines -Replace '\{version\}', $version
    Rewrite-File $file $lines $imports
  }
  
  # Imports
  
  $importFolder = Join-Path $targetDir "scripts\imports"
  
  foreach($path in $imports){
    $text = [IO.File]::ReadAllText($path)
    $text = [Regex]::Replace($text, '#import "(.*?)"', {
      $importPath = Join-Path $importFolder ($args[0].Groups[1].Value.Trim())
      $importStr = [IO.File]::ReadAllText($importPath).TrimEnd()
      
      if ($importStr[0] -eq '#'){
        $importStr = $importStr.Substring($importStr.IndexOf("`n") + 1)
      }
      
      return $importStr
    }, [System.Text.RegularExpressions.RegexOptions]::MultiLine)
    
    [IO.File]::WriteAllText($path, $text)
    Write-Host "Resolved" $path.Substring($targetDir.Length)
  }
  
  [IO.Directory]::Delete($importFolder, $True)
  
  # Finished
  
  $sw.Stop()
  Write-Host "------------------"
  Write-Host "Finished in" $([math]::Round($sw.Elapsed.TotalMilliseconds)) "ms"
  Write-Host "------------------"
  
}catch{
  Write-Host "Encountered an error while running PostBuild.ps1 on line" $_.InvocationInfo.ScriptLineNumber
  Write-Host $_
  Exit 1
}
