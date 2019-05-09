namespace TweetTest.Core.UrlUtils

open Xunit
open TweetLib.Core.Utils


module Check =
    type Result = UrlUtils.CheckResult

    [<Fact>]
    let ``accepts HTTP protocol`` () =
        Assert.Equal(Result.Fine, UrlUtils.Check("http://example.com"))

    [<Fact>]
    let ``accepts HTTPS protocol`` () =
        Assert.Equal(Result.Fine, UrlUtils.Check("https://example.com"))

    [<Fact>]
    let ``accepts FTP protocol`` () =
        Assert.Equal(Result.Fine, UrlUtils.Check("ftp://example.com"))

    [<Fact>]
    let ``accepts MAILTO protocol`` () =
        Assert.Equal(Result.Fine, UrlUtils.Check("mailto://someone@example.com"))

    [<Fact>]
    let ``accepts URL with port, path, query, and hash`` () =
        Assert.Equal(Result.Fine, UrlUtils.Check("http://www.example.co.uk:80/path?key=abc&array[]=5#hash"))

    [<Fact>]
    let ``accepts IPv4 address`` () =
        Assert.Equal(Result.Fine, UrlUtils.Check("http://127.0.0.1"))

    [<Fact>]
    let ``accepts IPv6 address`` () =
        Assert.Equal(Result.Fine, UrlUtils.Check("http://[2001:db8:0:0:0:ff00:42:8329]"))

    [<Fact>]
    let ``recognizes t.co as tracking URL`` () =
        Assert.Equal(Result.Tracking, UrlUtils.Check("http://t.co/12345"))

    [<Fact>]
    let ``rejects empty URL`` () =
        Assert.Equal(Result.Invalid, UrlUtils.Check(""))

    [<Fact>]
    let ``rejects missing protocol`` () =
        Assert.Equal(Result.Invalid, UrlUtils.Check("www.example.com"))

    [<Fact>]
    let ``rejects banned protocol`` () =
        Assert.Equal(Result.Invalid, UrlUtils.Check("file://example.com"))
        

module GetFileNameFromUrl =

    [<Fact>]
    let ``simple file URL returns file name`` () =
        Assert.Equal("index.html", UrlUtils.GetFileNameFromUrl("http://example.com/index.html"))

    [<Fact>]
    let ``file URL with query returns file name`` () =
        Assert.Equal("index.html", UrlUtils.GetFileNameFromUrl("http://example.com/index.html?version=2"))

    [<Fact>]
    let ``file URL w/o extension returns file name`` () =
        Assert.Equal("index", UrlUtils.GetFileNameFromUrl("http://example.com/index"))

    [<Fact>]
    let ``file URL with trailing dot returns file name with dot`` () =
        Assert.Equal("index.", UrlUtils.GetFileNameFromUrl("http://example.com/index."))

    [<Fact>]
    let ``root URL returns null`` () =
        Assert.Null(UrlUtils.GetFileNameFromUrl("http://example.com"))

    [<Fact>]
    let ``path URL returns null`` () =
        Assert.Null(UrlUtils.GetFileNameFromUrl("http://example.com/path/"))
