namespace TweetTest.Utils.Static.StringUtils

open System
open TweetLib.Utils.Static
open Xunit


module NullIfEmpty =

    [<Fact>]
    let ``null string returns null`` () =
        Assert.Null(StringUtils.NullIfEmpty(null))

    [<Fact>]
    let ``empty string returns null`` () =
        Assert.Null(StringUtils.NullIfEmpty(String.Empty))

    [<Fact>]
    let ``blank string returns a blank string`` () =
        Assert.Equal(" ", StringUtils.NullIfEmpty(" "))

    [<Fact>]
    let ``non-empty string returns a non-empty string`` () =
        Assert.Equal("abc", StringUtils.NullIfEmpty("abc"))


module SplitInTwo =

    [<Fact>]
    let ``empty string returns null`` () =
        Assert.Null(StringUtils.SplitInTwo("", '|'))

    [<Fact>]
    let ``non-empty string without search character returns null`` () =
        Assert.Null(StringUtils.SplitInTwo("abcdef", '|'))

    [<Fact>]
    let ``splitting into two non-empty parts works`` () =
        let result = StringUtils.SplitInTwo("abc|def", '|')
        Assert.NotNull(result)

        let struct (before, after) = result.Value
        Assert.Equal("abc", before)
        Assert.Equal("def", after)

    [<Fact>]
    let ``splitting into two empty parts works`` () =
        let result = StringUtils.SplitInTwo("|", '|')
        Assert.NotNull(result)

        let struct (before, after) = result.Value
        Assert.Equal("", before)
        Assert.Equal("", after)

    [<Fact>]
    let ``first found search character is used if multiple are present`` () =
        let result = StringUtils.SplitInTwo("abc|de|f", '|')
        Assert.NotNull(result)

        let struct (before, after) = result.Value
        Assert.Equal("abc", before)
        Assert.Equal("de|f", after)

    [<Fact>]
    let ``start index skips search characters preceding it`` () =
        let result = StringUtils.SplitInTwo("|bc|de|f", '|', 4)
        Assert.NotNull(result)

        let struct (before, after) = result.Value
        Assert.Equal("|bc|de", before)
        Assert.Equal("f", after)

    [<Fact>]
    let ``start index matching position of search character does not skip it`` () =
        let result = StringUtils.SplitInTwo("|bc|de|f", '|', 3)
        Assert.NotNull(result)

        let struct (before, after) = result.Value
        Assert.Equal("|bc", before)
        Assert.Equal("de|f", after)

    [<Fact>]
    let ``start index before start of string throws`` () =
        Assert.Throws<ArgumentOutOfRangeException>(fun () -> StringUtils.SplitInTwo("abc|def", '|', -1) |> ignore)

    [<Fact>]
    let ``start index after end of string throws`` () =
        Assert.Throws<ArgumentOutOfRangeException>(fun () -> StringUtils.SplitInTwo("abc|def", '|', 8) |> ignore)


module ExtractBefore =

    [<Fact>]
    let ``empty string returns empty string`` () =
        Assert.Equal("", StringUtils.ExtractBefore("", ' '))

    [<Fact>]
    let ``non-empty string without search character returns input string`` () =
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

    [<Fact>]
    let ``start index before start of string throws`` () =
        Assert.Throws<ArgumentOutOfRangeException>(fun () -> StringUtils.ExtractBefore("abc def", ' ', -1) |> ignore)

    [<Fact>]
    let ``start index after end of string throws`` () =
        Assert.Throws<ArgumentOutOfRangeException>(fun () -> StringUtils.ExtractBefore("abc def", ' ', 8) |> ignore)


module ParseInts =

    [<Fact>]
    let ``empty string returns empty collection`` () =
        Assert.Empty(StringUtils.ParseInts("", ','))

    [<Fact>]
    let ``single integer is parsed correctly`` () =
        Assert.Equal([ 1 ], StringUtils.ParseInts("1", ','))

    [<Fact>]
    let ``multiple integers are parsed correctly`` () =
        Assert.Equal([ -3 .. 3 ], StringUtils.ParseInts("-3,-2,-1,0,1,2,3", ','))

    [<Fact>]
    let ``neighbouring delimiters are discarded`` () =
        Assert.Equal([ 1 .. 4 ], StringUtils.ParseInts(",,1,,,2,3,,4,,,", ','))

    [<Fact>]
    let ``invalid format throws`` () =
        Assert.Throws<FormatException>(fun () -> StringUtils.ParseInts("1,2,a", ',') |> ignore)


module ConvertPascalCaseToScreamingSnakeCase =

    [<Fact>]
    let ``converts one word`` () =
        Assert.Equal("HELP", StringUtils.ConvertPascalCaseToScreamingSnakeCase("Help"))

    [<Fact>]
    let ``converts two words`` () =
        Assert.Equal("HELP_ME", StringUtils.ConvertPascalCaseToScreamingSnakeCase("HelpMe"))

    [<Fact>]
    let ``converts three words`` () =
        Assert.Equal("HELP_ME_PLEASE", StringUtils.ConvertPascalCaseToScreamingSnakeCase("HelpMePlease"))

    [<Fact>]
    let ``converts one uppercase abbreviation`` () =
        Assert.Equal("HTML_CODE", StringUtils.ConvertPascalCaseToScreamingSnakeCase("HTMLCode"))

    [<Fact>]
    let ``converts many uppercase abbreviations`` () =
        Assert.Equal("I_LIKE_HTML_AND_CSS", StringUtils.ConvertPascalCaseToScreamingSnakeCase("ILikeHTMLAndCSS"))


module ConvertRot13 =

    [<Fact>]
    let ``ignores digits and special characters`` () =
        Assert.Equal("<123'456.789>", StringUtils.ConvertRot13("<123'456.789>"))

    [<Fact>]
    let ``converts lowercase letters correctly`` () =
        Assert.Equal("nopqrstuvwxyzabcdefghijklm", StringUtils.ConvertRot13("abcdefghijklmnopqrstuvwxyz"))

    [<Fact>]
    let ``converts uppercase letters correctly`` () =
        Assert.Equal("NOPQRSTUVWXYZABCDEFGHIJKLM", StringUtils.ConvertRot13("ABCDEFGHIJKLMNOPQRSTUVWXYZ"))

    [<Fact>]
    let ``converts mixed character types correctly`` () =
        Assert.Equal("Uryyb, jbeyq! :)", StringUtils.ConvertRot13("Hello, world! :)"))
