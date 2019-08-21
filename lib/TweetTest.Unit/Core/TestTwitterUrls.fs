namespace TweetTest.Core.TestTwitterUrls

open Xunit
open TweetLib.Core.Features.Twitter


module Check =
    type Result = TwitterUrls.UrlType

    [<Fact>]
    let ``accepts HTTP protocol`` () =
        Assert.Equal(Result.Fine, TwitterUrls.Check("http://example.com"))

    [<Fact>]
    let ``accepts HTTPS protocol`` () =
        Assert.Equal(Result.Fine, TwitterUrls.Check("https://example.com"))

    [<Fact>]
    let ``accepts FTP protocol`` () =
        Assert.Equal(Result.Fine, TwitterUrls.Check("ftp://example.com"))

    [<Fact>]
    let ``accepts MAILTO protocol`` () =
        Assert.Equal(Result.Fine, TwitterUrls.Check("mailto://someone@example.com"))

    [<Fact>]
    let ``accepts URL with port, path, query, and hash`` () =
        Assert.Equal(Result.Fine, TwitterUrls.Check("http://www.example.co.uk:80/path?key=abc&array[]=5#hash"))

    [<Fact>]
    let ``accepts IPv4 address`` () =
        Assert.Equal(Result.Fine, TwitterUrls.Check("http://127.0.0.1"))

    [<Fact>]
    let ``accepts IPv6 address`` () =
        Assert.Equal(Result.Fine, TwitterUrls.Check("http://[2001:db8:0:0:0:ff00:42:8329]"))

    [<Fact>]
    let ``recognizes t.co as tracking URL`` () =
        Assert.Equal(Result.Tracking, TwitterUrls.Check("http://t.co/12345"))

    [<Fact>]
    let ``rejects empty URL`` () =
        Assert.Equal(Result.Invalid, TwitterUrls.Check(""))

    [<Fact>]
    let ``rejects missing protocol`` () =
        Assert.Equal(Result.Invalid, TwitterUrls.Check("www.example.com"))

    [<Fact>]
    let ``rejects banned protocol`` () =
        Assert.Equal(Result.Invalid, TwitterUrls.Check("file://example.com"))
        

module GetFileNameFromUrl =

    [<Fact>]
    let ``simple file URL returns file name`` () =
        Assert.Equal("index.html", TwitterUrls.GetFileNameFromUrl("http://example.com/index.html"))

    [<Fact>]
    let ``file URL with query returns file name`` () =
        Assert.Equal("index.html", TwitterUrls.GetFileNameFromUrl("http://example.com/index.html?version=2"))

    [<Fact>]
    let ``file URL w/o extension returns file name`` () =
        Assert.Equal("index", TwitterUrls.GetFileNameFromUrl("http://example.com/index"))

    [<Fact>]
    let ``file URL with trailing dot returns file name with dot`` () =
        Assert.Equal("index.", TwitterUrls.GetFileNameFromUrl("http://example.com/index."))

    [<Fact>]
    let ``root URL returns null`` () =
        Assert.Null(TwitterUrls.GetFileNameFromUrl("http://example.com"))

    [<Fact>]
    let ``path URL returns null`` () =
        Assert.Null(TwitterUrls.GetFileNameFromUrl("http://example.com/path/"))


module GetMediaLink_Default =
    let getMediaLinkDefault url = TwitterUrls.GetMediaLink(url, ImageQuality.Default)
    let domain = "https://pbs.twimg.com"

    [<Fact>]
    let ``does not modify URL w/o extension`` () =
        Assert.Equal(domain+"/media/123", getMediaLinkDefault(domain+"/media/123"))

    [<Fact>]
    let ``does not modify URL w/o quality suffix`` () =
        Assert.Equal(domain+"/media/123.jpg", getMediaLinkDefault(domain+"/media/123.jpg"))

    [<Fact>]
    let ``does not modify URL with quality suffix`` () =
        Assert.Equal(domain+"/media/123.jpg:small", getMediaLinkDefault(domain+"/media/123.jpg:small"))


module GetMediaLink_Orig =
    let getMediaLinkOrig url = TwitterUrls.GetMediaLink(url, ImageQuality.Best)
    let domain = "https://pbs.twimg.com"
    
    [<Fact>]
    let ``appends :orig to valid URL w/o quality suffix`` () =
        Assert.Equal(domain+"/media/123.jpg:orig", getMediaLinkOrig(domain+"/media/123.jpg"))

    [<Fact>]
    let ``rewrites :orig into valid URL with quality suffix`` () =
        Assert.Equal(domain+"/media/123.jpg:orig", getMediaLinkOrig(domain+"/media/123.jpg:small"))
        
    [<Fact>]
    let ``does not modify unknown URL w/o quality suffix`` () =
        Assert.Equal(domain+"/profile_images/123.jpg", getMediaLinkOrig(domain+"/profile_images/123.jpg"))
        
    [<Fact>]
    let ``rewrites :orig into unknown URL with quality suffix`` () =
        Assert.Equal(domain+"/profile_images/123.jpg:orig", getMediaLinkOrig(domain+"/profile_images/123.jpg:small"))


module GetImageFileName =

    [<Fact>]
    let ``extracts file name from URL w/o quality suffix`` () =
        Assert.Equal("test.jpg", TwitterUrls.GetImageFileName("http://example.com/test.jpg"))
        
    [<Fact>]
    let ``extracts file name from URL with quality suffix`` () =
        Assert.Equal("test.jpg", TwitterUrls.GetImageFileName("http://example.com/test.jpg:orig"))

    [<Fact>]
    let ``extracts file name from URL with a port`` () =
        Assert.Equal("test.jpg", TwitterUrls.GetImageFileName("http://example.com:80/test.jpg"))


[<Collection("RegexAccount")>]
module RegexAccount_IsMatch =
    let isMatch = TwitterUrls.RegexAccount.IsMatch

    [<Fact>]
    let ``accepts HTTP protocol`` () =
        Assert.True(isMatch("http://twitter.com/chylexmc"))

    [<Fact>]
    let ``accepts HTTPS protocol`` () =
        Assert.True(isMatch("https://twitter.com/chylexmc"))

    [<Fact>]
    let ``accepts trailing slash`` () =
        Assert.True(isMatch("https://twitter.com/chylexmc/"))

    [<Fact>]
    let ``rejects URL with query`` () =
        Assert.False(isMatch("https://twitter.com/chylexmc?query"))

    [<Fact>]
    let ``rejects URL with extra path`` () =
        Assert.False(isMatch("https://twitter.com/chylexmc/status/123"))

    [<Theory>]
    [<InlineData("signup")>]
    [<InlineData("tos")>]
    [<InlineData("privacy")>]
    [<InlineData("search")>]
    [<InlineData("search?query")>]
    [<InlineData("search-home")>]
    [<InlineData("search-advanced")>]
    let ``rejects reserved page names`` (name: string) =
        Assert.False(isMatch("https://twitter.com/"+name))
        
    [<Theory>]
    [<InlineData("tosser")>]
    [<InlineData("searching")>]
    let ``accepts accounts starting with reserved page names`` (name: string) =
        Assert.True(isMatch("https://twitter.com/"+name))

        
[<Collection("RegexAccount")>]
module RegexAccount_Match =
    let extract str = TwitterUrls.RegexAccount.Match(str).Groups.[1].Value

    [<Fact>]
    let ``extracts account name from simple URL`` () =
        Assert.Equal("_abc_DEF_123", extract("https://twitter.com/_abc_DEF_123"))

    [<Fact>]
    let ``extracts account name from URL with trailing slash`` () =
        Assert.Equal("_abc_DEF_123", extract("https://twitter.com/_abc_DEF_123/"))
