@ECHO OFF

IF EXIST "post_build.exe" (
  DEL "post_build.exe"
)

IF NOT EXIST %1 (
  ECHO fsc.exe not found
  EXIT 1
)

%1 --standalone --deterministic --preferreduilang:en-US --platform:x86 --target:exe --out:post_build.exe "%~dp0..\Resources\PostBuild.fsx"
