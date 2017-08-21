Param([Parameter(Mandatory = $True, Position = 1)][string] $dir)
$ErrorActionPreference = "Stop"

Set-Location $dir

ForEach($file in Get-ChildItem -Include *.js, *.html -Recurse){
  $lines = Get-Content -Path $file.FullName
  $lines = ($lines | % { $_.TrimStart() }) -Replace '^(.*?)((?<=^|[;{}()] )//\s.*)?$', '$1'
  $lines | Where { $_ -ne '' } | Set-Content -Path $file.FullName

  Write-Host "Processed" $file.FullName.Substring($dir.Length)
}
