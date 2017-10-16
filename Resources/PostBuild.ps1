Param([Parameter(Mandatory = $True, Position = 1)][string] $dir)
$ErrorActionPreference = "Stop"

Set-Location $dir

function Check-Carriage-Return{
  Param([Parameter(Mandatory = $True, Position = 1)] $fname)
  
  $file = @(Get-ChildItem -Include $fname -Recurse)[0]
  
  if ((Get-Content -Path $file.FullName -Raw).Contains("`r")){
    Throw "$fname cannot contain carriage return"
  }
  
  Write-Host "Verified" $file.FullName.Substring($dir.Length)
}

function Rewrite-File{
  [CmdletBinding()]
  Param([Parameter(Mandatory = $True, ValueFromPipeline = $True)][array] $lines, [Parameter(Mandatory = $True, Position = 1)] $file)
  
  $lines | Where { $_ -ne '' } | Set-Content -Path $file.FullName
  Write-Host "Processed" $file.FullName.Substring($dir.Length)
}

try{
  Check-Carriage-Return("emoji-ordering.txt")

  ForEach($file in Get-ChildItem -Filter *.js -Exclude configuration.default.js -Recurse){
    $lines = Get-Content -Path $file.FullName
    $lines = $lines | % { $_.TrimStart() }
    $lines = $lines -Replace '^(.*?)((?<=^|[;{}()])\s?//(?:\s.*|$))?$', '$1'
    $lines = $lines -Replace '(?<!\w)return(\s.*?)? if (.*?);', 'if ($2)return$1;'
    ,$lines | Rewrite-File $file
  }

  ForEach($file in Get-ChildItem -Filter *.css -Recurse){
    $lines = Get-Content -Path $file.FullName
    $lines = $lines -Replace '\s*/\*.*?\*/', ''
    $lines = $lines -Replace '^\s+(.+):\s?(.+?)(?:\s?(!important))?;$', '$1:$2$3;'
    $lines = $lines -Replace '^(\S.*?) {$', '$1{'
    @(($lines | Where { $_ -ne '' }) -Join ' ') | Rewrite-File $file
  }

  ForEach($file in Get-ChildItem -Filter *.html -Recurse){
    $lines = Get-Content -Path $file.FullName
    ,($lines | % { $_.TrimStart() }) | Rewrite-File $file
  }
}catch{
  Write-Host "Encountered an error while running PostBuild.ps1 on line" $_.InvocationInfo.ScriptLineNumber
  Write-Host $_
  Exit 1
}
