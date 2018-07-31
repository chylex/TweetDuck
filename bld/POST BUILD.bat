@ECHO OFF

DEL "post_build.exe"

SET fsc="%PROGRAMFILES(x86)%\Microsoft SDKs\F#\10.1\Framework\v4.0\fsc.exe"

IF NOT EXIST %fsc% (
  SET fsc="%PROGRAMFILES%\Microsoft SDKs\F#\10.1\Framework\v4.0\fsc.exe"
)

IF NOT EXIST %fsc% (
  ECHO fsc.exe not found
  EXIT 1
)

%fsc% --standalone --deterministic --preferreduilang:en-US --platform:x86 --target:exe --out:post_build.exe "%~dp0..\Resources\PostBuild.fsx"
