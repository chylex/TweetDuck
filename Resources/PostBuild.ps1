Param([Parameter(Mandatory = $True, Position = 1)][string] $dir)
$ErrorActionPreference = "Stop"

Set-Location $dir

function Rewrite-File{
  [CmdletBinding()]
  Param([Parameter(Mandatory = $True, ValueFromPipeline = $True)][array] $lines, [Parameter(Mandatory = $True, Position = 1)] $file)
  
  $lines | Where { $_ -ne '' } | Set-Content -Path $file.FullName
  Write-Host "Processed" $file.FullName.Substring($dir.Length)
}

ForEach($file in Get-ChildItem -Include *.js -Recurse){
  $lines = Get-Content -Path $file.FullName
  $lines = ($lines | % { $_.TrimStart() }) -Replace '^(.*?)((?<=^|[;{}()])\s?//(?:\s.*|$))?$', '$1' -Replace '(?<!\w)return(\s.*?)? if (.*?);', 'if ($2)return$1;'
  ,$lines | Rewrite-File $file
}

ForEach($file in Get-ChildItem -Include *.html -Recurse){
  $lines = Get-Content -Path $file.FullName
  ,($lines | % { $_.TrimStart() }) | Rewrite-File $file
}
