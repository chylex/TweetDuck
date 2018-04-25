Param(
  [Parameter(Mandatory = $True, Position = 1)][string] $targetDir,
  [Parameter(Mandatory = $True, Position = 2)][string] $projectDir,
  [Parameter(Position = 3)][string] $version = ""
)

$ErrorActionPreference = "Stop"

try{
  Write-Host "------------------------------"
  
  if ($version.Equals("")){
    $version = (Get-Item (Join-Path $targetDir "TweetDuck.exe")).VersionInfo.FileVersion
  }
  
  Write-Host "TweetDuck version" $version
  
  Write-Host "------------------------------"
  
  # Cleanup
  
  if (Test-Path (Join-Path $targetDir "locales")){
    Remove-Item -Path (Join-Path $targetDir "locales\*.pak") -Exclude "en-US.pak"
  }
  
  # Copy resources
  
  Copy-Item (Join-Path $projectDir "bld\Resources\CEFSHARP-LICENSE.txt") -Destination $targetDir -Force
  Copy-Item (Join-Path $projectDir "LICENSE.md") -Destination (Join-Path $targetDir "LICENSE.txt") -Force      
  
  New-Item -ItemType directory -Path $targetDir -Name "scripts" | Out-Null
  New-Item -ItemType directory -Path $targetDir -Name "plugins" | Out-Null
  New-Item -ItemType directory -Path $targetDir -Name "plugins\official" | Out-Null
  New-Item -ItemType directory -Path $targetDir -Name "plugins\user" | Out-Null
  
  Copy-Item (Join-Path $projectDir "Resources\Scripts\*") -Destination (Join-Path $targetDir "scripts") -Recurse
  Copy-Item (Join-Path $projectDir "Resources\Plugins\*") -Exclude ".debug", "emoji-instructions.txt" -Destination (Join-Path $targetDir "plugins\official") -Recurse
  
  # Post processing
  
  function Check-Carriage-Return{
    Param(
      [Parameter(Mandatory = $True, Position = 1)] $fname
    )
    
    $file = @(Get-ChildItem -Path $targetDir -Include $fname -Recurse)[0]
    
    if ((Get-Content -Path $file.FullName -Raw).Contains("`r")){
      Throw "$fname cannot contain carriage return"
    }
    
    Write-Host "Verified" $file.FullName.Substring($targetDir.Length)
  }
  
  function Rewrite-File{
    [CmdletBinding()]
    Param(
      [Parameter(Mandatory = $True, ValueFromPipeline = $True)][array] $lines,
      [Parameter(Mandatory = $True, Position = 1)] $file
    )
    
    $relativePath = $file.FullName.Substring($targetDir.Length)
    
    if ($relativePath.StartsWith("scripts\")){
      $lines = (,("#" + $version) + $lines)
    }
    
    $lines | Where { $_ -ne '' } | Set-Content -Path $file.FullName
    Write-Host "Processed" $relativePath
  }
  
  Check-Carriage-Return("emoji-ordering.txt")

  ForEach($file in Get-ChildItem -Path $targetDir -Filter "*.js" -Exclude "configuration.default.js" -Recurse){
    $lines = Get-Content -Path $file.FullName
    $lines = $lines | % { $_.TrimStart() }
    $lines = $lines -Replace '^(.*?)((?<=^|[;{}()])\s?//(?:\s.*|$))?$', '$1'
    $lines = $lines -Replace '(?<!\w)return(\s.*?)? if (.*?);', 'if ($2)return$1;'
    ,$lines | Rewrite-File $file
  }

  ForEach($file in Get-ChildItem -Path $targetDir -Filter "*.css" -Recurse){
    $lines = Get-Content -Path $file.FullName
    $lines = $lines -Replace '\s*/\*.*?\*/', ''
    $lines = $lines -Replace '^\s+(.+):\s?(.+?)(?:\s?(!important))?;$', '$1:$2$3;'
    $lines = $lines -Replace '^(\S.*?) {$', '$1{'
    @(($lines | Where { $_ -ne '' }) -Join ' ') | Rewrite-File $file
  }

  ForEach($file in Get-ChildItem -Path $targetDir -Filter "*.html" -Recurse){
    $lines = Get-Content -Path $file.FullName
    ,($lines | % { $_.TrimStart() }) | Rewrite-File $file
  }
  
  Write-Host "------------------------------"
  ForEach($file in Get-ChildItem -Path $targetDir -Filter "*.meta" -Recurse){
    $lines = Get-Content -Path $file.FullName
    $lines = $lines -Replace '\{version\}', $version
    ,$lines | Rewrite-File $file
  }
}catch{
  Write-Host "Encountered an error while running PostBuild.ps1 on line" $_.InvocationInfo.ScriptLineNumber
  Write-Host $_
  Exit 1
}
