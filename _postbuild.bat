del "bin\x86\Release Dick\*.xml"
del "bin\x86\Release Dick\devtools_resources.pak"
del "bin\x86\Release Duck\*.xml"
del "bin\x86\Release Duck\devtools_resources.pak"

del "bin\x86\Release Dick\TweetDick.Browser.exe"
ren "bin\x86\Release Dick\CefSharp.BrowserSubprocess.exe" "TweetDick.Browser.exe"
del "bin\x86\Release Duck\TweetDuck.Browser.exe"
ren "bin\x86\Release Duck\CefSharp.BrowserSubprocess.exe" "TweetDuck.Browser.exe"