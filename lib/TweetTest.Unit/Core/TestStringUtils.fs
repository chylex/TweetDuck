namespace Unit.Core.StringUtils

open Xunit
open TweetDuck.Core.Utils


module ExtractBefore =

    [<Fact>]
    let ``empty input string returns empty string`` () =
        Assert.Equal("", StringUtils.ExtractBefore("", ' '))

    [<Fact>]
    let ``input string w/o searched char returns input string`` () =
        Assert.Equal("abc", StringUtils.ExtractBefore("abc", ' '))

    [<Fact>]
    let ``finding searched char returns everything before it`` () =
        Assert.Equal("abc", StringUtils.ExtractBefore("abc def ghi", ' '))

    [<Theory>]
    [<InlineData(0, "abc")>]
    [<InlineData(3, "abc")>]
    [<InlineData(4, "abc def")>]
    [<InlineData(7, "abc def")>]
    [<InlineData(8, "abc def ghi")>]
    let ``start index works`` (startIndex: int, expectedValue: string) =
        Assert.Equal(expectedValue, StringUtils.ExtractBefore("abc def ghi", ' ', startIndex))
        

module ParseInts =
    open System
    
    [<Fact>]
    let ``empty input string returns empty collection`` () =
        Assert.Empty(StringUtils.ParseInts("", ','))
    
    [<Fact>]
    let ``single integer is parsed correctly`` () =
        Assert.Equal([ 1 ], StringUtils.ParseInts("1", ','))
    
    [<Fact>]
    let ``multiple integers are parsed correctly`` () =
        Assert.Equal([ -3 .. 3 ], StringUtils.ParseInts("-3,-2,-1,0,1,2,3", ','))
    
    [<Fact>]
    let ``excessive delimiters are discarded`` () =
        Assert.Equal([ 1 .. 4 ], StringUtils.ParseInts(",,1,,,2,3,,4,,,", ','))

    [<Fact>]
    let ``invalid format throws exception`` () =
        Assert.Throws<FormatException>(fun () -> StringUtils.ParseInts("1,2,a", ',') |> ignore)


module ConvertPascalCaseToScreamingSnakeCase =

    [<Fact>]
    let ``converts one word`` () =
        Assert.Equal("HELP", StringUtils.ConvertPascalCaseToScreamingSnakeCase("Help"))

    [<Fact>]
    let ``converts two words`` () =
        Assert.Equal("HELP_ME", StringUtils.ConvertPascalCaseToScreamingSnakeCase("HelpMe"))

    [<Fact>]
    let ``converts many words`` () =
        Assert.Equal("HELP_ME_PLEASE", StringUtils.ConvertPascalCaseToScreamingSnakeCase("HelpMePlease"))

    [<Fact>]
    let ``converts one uppercase abbreviation`` () =
        Assert.Equal("HTML_CODE", StringUtils.ConvertPascalCaseToScreamingSnakeCase("HTMLCode"))

    [<Fact>]
    let ``converts many uppercase abbreviations`` () =
        Assert.Equal("I_LIKE_HTML_AND_CSS", StringUtils.ConvertPascalCaseToScreamingSnakeCase("ILikeHTMLAndCSS"))


// TODO add new tests
