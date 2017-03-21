if exist "..\bin\x86\Release\CefSharp.BrowserSubprocess.exe" (
  del "..\bin\x86\Release\TweetDuck.Browser.exe"
  ren "..\bin\x86\Release\CefSharp.BrowserSubprocess.exe" "TweetDuck.Browser.exe"
)

start "" /B "ISCC.exe" /Q "gen_full.iss"
start "" /B "ISCC.exe" /Q "gen_port.iss"
start "" /B "ISCC.exe" /Q "gen_upd.iss"
