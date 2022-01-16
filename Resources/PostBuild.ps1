Param(
  [Parameter(Mandatory = $True, Position = 1)][string] $targetDir,
  [Parameter(Position = 2)][string] $version = ""
)

$ErrorActionPreference = "Stop"

try {
  $sw = [Diagnostics.Stopwatch]::StartNew()

  if ($version.Equals("")) {
    $version = (Get-Item (Join-Path $targetDir "TweetDuck.exe")).VersionInfo.FileVersion
  }

  Write-Host "--------------------------"
  Write-Host "TweetDuck version" $version
  Write-Host "--------------------------"

  # Helper functions

  function Check-Carriage-Return {
    Param([Parameter(Mandatory = $True, Position = 1)] $file)

    if (!(Test-Path $file)) {
      Throw "$file does not exist"
    }

    if ((Get-Content -Path $file -Raw).Contains("`r")) {
      Throw "$file must not have any carriage return characters"
    }

    Write-Host "Verified" $file.Substring($targetDir.Length)
  }

  function Rewrite-File {
    Param([Parameter(Mandatory = $True, Position = 1)] $file,
          [Parameter(Mandatory = $True, Position = 2)] $lines)

    $relativePath = $file.FullName.Substring($targetDir.Length)
    [IO.File]::WriteAllLines($file.FullName, $lines)
    Write-Host "Processed" $relativePath
  }

  # Validation

  Check-Carriage-Return (Join-Path $targetDir "plugins\official\emoji-keyboard\emoji-ordering.txt")

  # Processing

  foreach ($file in Get-ChildItem -Path (Join-Path $targetDir "plugins") -Filter "*.meta" -Recurse) {
    $lines = [IO.File]::ReadLines($file.FullName)
    $lines = $lines -Replace '\{version\}', $version
    Rewrite-File $file $lines
  }

  # Finished

  $sw.Stop()
  Write-Host "------------------"
  Write-Host "Finished in" $([math]::Round($sw.Elapsed.TotalMilliseconds)) "ms"
  Write-Host "------------------"

} catch {
  Write-Host "Encountered an error while running PostBuild.ps1 on line" $_.InvocationInfo.ScriptLineNumber
  Write-Host $_
  Exit 1
}
