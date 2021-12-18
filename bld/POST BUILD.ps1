if ([IO.File]::Exists("post_build.exe")) {
  [IO.File]::Delete("post_build.exe");
}

$fs = $args[0];
$fsc = "";

if ([IO.File]::Exists("$fs\fsc.exe")) {
  $fsc = "$fs\fsc.exe";
}

if ([IO.File]::Exists("$fs\Tools\fsc.exe")) {
  $fsc = "$fs\Tools\fsc.exe";
}

if ($fsc -eq "") {
  Write-Host "fsc.exe not found"
  $Host.SetShouldExit(1);
  exit
}

& $fsc --standalone --deterministic --preferreduilang:en-US --platform:x86 --target:exe --out:post_build.exe "$PSScriptRoot\..\Resources\PostBuild.fsx"
