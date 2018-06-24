namespace Unit.Core.TwitterUtils

open Xunit
open TweetDuck.Core.Utils


[<Collection("RegexAccount")>]
module RegexAccount_IsMatch =
    let isMatch = TwitterUtils.RegexAccount.IsMatch

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
    let extract str = TwitterUtils.RegexAccount.Match(str).Groups.[1].Value

    [<Fact>]
    let ``extracts account name from simple URL`` () =
        Assert.Equal("_abc_DEF_123", extract("https://twitter.com/_abc_DEF_123"))

    [<Fact>]
    let ``extracts account name from URL with trailing slash`` () =
        Assert.Equal("_abc_DEF_123", extract("https://twitter.com/_abc_DEF_123/"))


module GetMediaLink_Default =
    let getMediaLinkDefault url = TwitterUtils.GetMediaLink(url, TwitterUtils.ImageQuality.Default)
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
    let getMediaLinkOrig url = TwitterUtils.GetMediaLink(url, TwitterUtils.ImageQuality.Orig)
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
        Assert.Equal("test.jpg", TwitterUtils.GetImageFileName("http://example.com/test.jpg"))
        
    [<Fact>]
    let ``extracts file name from URL with quality suffix`` () =
        Assert.Equal("test.jpg", TwitterUtils.GetImageFileName("http://example.com/test.jpg:orig"))

    [<Fact>]
    let ``extracts file name from URL with a port`` () =
        Assert.Equal("test.jpg", TwitterUtils.GetImageFileName("http://example.com:80/test.jpg"))
