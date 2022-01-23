namespace TweetTest.Browser.CEF.Utils.CefUtils

open TweetLib.Browser.CEF.Utils
open Xunit


module ParseCommandLineArguments =

    [<Fact>]
    let ``returns empty args if input string is empty`` () =
        Assert.Equal(0, CefUtils.ParseCommandLineArguments("").Count)

    [<Fact>]
    let ``returns empty args if input string is whitespace`` () =
        Assert.Equal(0, CefUtils.ParseCommandLineArguments(" \r\n \t").Count)

    [<Fact>]
    let ``reads values correctly`` () =
        let args = CefUtils.ParseCommandLineArguments("--first-value=10 --second-value=\"long string with spaces\"")

        Assert.Equal(2, args.Count)
        Assert.Equal("10", args.GetValue("first-value"))
        Assert.Equal("long string with spaces", args.GetValue("second-value"))

    [<Fact>]
    let ``reads flags as valued properties with values of 1`` () =
        let args = CefUtils.ParseCommandLineArguments("--first-flag-as-value --second-flag-as-value")

        Assert.Equal(2, args.Count)
        Assert.Equal("1", args.GetValue("first-flag-as-value"))
        Assert.Equal("1", args.GetValue("second-flag-as-value"))

    [<Fact>]
    let ``reads complex string with whitespace correctly`` () =
        let args = CefUtils.ParseCommandLineArguments("\t--first-value=55.5\r\n--first-flag-as-value\r\n --second-value=\"long string\"\t--second-flag-as-value ")

        Assert.Equal(4, args.Count)
        Assert.Equal("55.5", args.GetValue("first-value"))
        Assert.Equal("long string", args.GetValue("second-value"))
        Assert.Equal("1", args.GetValue("first-flag-as-value"))
        Assert.Equal("1", args.GetValue("second-flag-as-value"))
