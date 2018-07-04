namespace Unit.Data.CommandLineArgs

open Xunit
open TweetDuck.Data


type _TestData =

    static member empty
        with get() = CommandLineArgs()

    static member flags
        with get() =
            let args = CommandLineArgs()
            args.AddFlag("flag1")
            args.AddFlag("flag2")
            args.AddFlag("flag3")
            args

    static member values
        with get() =
            let args = CommandLineArgs()
            args.SetValue("val1", "hello")
            args.SetValue("val2", "world")
            args

    static member mixed
        with get() =
            let args = CommandLineArgs()
            args.AddFlag("flag1")
            args.AddFlag("flag2")
            args.AddFlag("flag3")
            args.SetValue("val1", "hello")
            args.SetValue("val2", "world")
            args

    static member duplicate
        with get() =
            let args = CommandLineArgs()
            args.AddFlag("duplicate")
            args.SetValue("duplicate", "value")
            args


module Count =

    [<Fact>]
    let ``counts nothing correctly`` () =
        Assert.Equal(0, _TestData.empty.Count)

    [<Fact>]
    let ``counts flags correctly`` () =
        Assert.Equal(3, _TestData.flags.Count)

    [<Fact>]
    let ``counts values correctly`` () =
        Assert.Equal(2, _TestData.values.Count)

    [<Fact>]
    let ``counts mixed flags and values correctly`` () =
        Assert.Equal(5, _TestData.mixed.Count)


module Flags =
        
    [<Theory>]
    [<InlineData("flag1")>]
    [<InlineData("flag2")>]
    [<InlineData("flag3")>]
    let ``HasFlag returns false if flag is missing`` (flag: string) =
        Assert.False(_TestData.empty.HasFlag(flag))
        Assert.False(_TestData.values.HasFlag(flag))
        
    [<Theory>]
    [<InlineData("val1")>]
    [<InlineData("val2")>]
    let ``HasFlag returns false if the name only specifies a key`` (flag: string) =
        Assert.False(_TestData.values.HasFlag(flag))
        
    [<Fact>]
    let ``HasFlag returns true if the name specifies both a flag and a value key`` () =
        Assert.True(_TestData.duplicate.HasFlag("duplicate"))

    [<Theory>]
    [<InlineData("flag1")>]
    [<InlineData("flag2")>]
    [<InlineData("flag3")>]
    let ``HasFlag returns true if flag is present`` (flag: string) =
        Assert.True(_TestData.flags.HasFlag(flag))

    [<Theory>]
    [<InlineData("FLAG1")>]
    [<InlineData("FlAg1")>]
    let ``HasFlag is case-insensitive`` (flag: string) =
        Assert.True(_TestData.flags.HasFlag(flag))

    [<Fact>]
    let ``AddFlag adds new flag`` () =
        let args = _TestData.flags
        args.AddFlag("flag4")
        
        Assert.Equal(4, args.Count)
        Assert.True(args.HasFlag("flag4"))

    [<Fact>]
    let ``AddFlag does nothing if flag is already present`` () =
        let args = _TestData.flags
        args.AddFlag("flag1")
        
        Assert.Equal(3, args.Count)

    [<Theory>]
    [<InlineData("flag1")>]
    [<InlineData("flag2")>]
    [<InlineData("flag3")>]
    let ``RemoveFlag removes existing flag`` (flag: string) =
        let args = _TestData.flags
        args.RemoveFlag(flag)
        
        Assert.Equal(2, args.Count)
        Assert.False(args.HasFlag(flag))

    [<Theory>]
    [<InlineData("FLAG1")>]
    [<InlineData("FlAg1")>]
    let ``RemoveFlag is case-insensitive`` (flag: string) =
        let args = _TestData.flags
        args.RemoveFlag(flag)
        
        Assert.Equal(2, args.Count)
        Assert.False(args.HasFlag(flag))

    [<Fact>]
    let ``RemoveFlag does nothing if flag is missing`` () =
        let args = _TestData.flags
        args.RemoveFlag("missing")
        
        Assert.Equal(3, args.Count)


module Values =
 
    [<Theory>]
    [<InlineData("val1")>]
    [<InlineData("val2")>]
    let ``HasValue returns false if key is missing`` (key: string) =
        Assert.False(_TestData.empty.HasValue(key))
        Assert.False(_TestData.flags.HasValue(key))
        
    [<Theory>]
    [<InlineData("flag1")>]
    [<InlineData("flag2")>]
    [<InlineData("flag3")>]
    let ``HasValue returns false if the name specifies a flag`` (key: string) =
        Assert.False(_TestData.flags.HasValue(key))
        
    [<Fact>]
    let ``HasValue returns true if the name specifies both a flag and a value key`` () =
        Assert.True(_TestData.duplicate.HasValue("duplicate"))

    [<Theory>]
    [<InlineData("val1")>]
    [<InlineData("val2")>]
    let ``HasValue returns true if key is present`` (key: string) =
        Assert.True(_TestData.values.HasValue(key))

    [<Theory>]
    [<InlineData("VAL1")>]
    [<InlineData("VaL1")>]
    let ``HasValue is case-insensitive`` (key: string) =
        Assert.True(_TestData.values.HasValue(key))

    [<Theory>]
    [<InlineData("val1", "hello")>]
    [<InlineData("val2", "world")>]
    let ``GetValue returns correct value if key is present`` (key: string, expectedValue: string) =
        Assert.Equal(expectedValue, _TestData.values.GetValue(key, ""))

    [<Theory>]
    [<InlineData("VAL1", "hello")>]
    [<InlineData("VaL1", "hello")>]
    let ``GetValue is case-insensitive`` (key: string, expectedValue: string) =
        Assert.Equal(expectedValue, _TestData.values.GetValue(key, ""))

    [<Fact>]
    let ``GetValue returns default value if key is missing`` () =
        Assert.Equal("oh no", _TestData.values.GetValue("missing", "oh no"))

    [<Fact>]
    let ``SetValue adds new value`` () =
        let args = _TestData.values
        args.SetValue("val3", "this is nice")
        
        Assert.Equal(3, args.Count)
        Assert.Equal("this is nice", args.GetValue("val3", ""))

    [<Fact>]
    let ``SetValue replaces existing value`` () =
        let args = _TestData.values
        args.SetValue("val2", "mom")
        
        Assert.Equal(2, args.Count)
        Assert.Equal("mom", args.GetValue("val2", ""))

    [<Theory>]
    [<InlineData("val1")>]
    [<InlineData("val2")>]
    let ``RemoveValue removes existing key`` (key: string) =
        let args = _TestData.values
        args.RemoveValue(key)
        
        Assert.Equal(1, args.Count)
        Assert.False(args.HasValue(key))

    [<Theory>]
    [<InlineData("VAL1")>]
    [<InlineData("VaL1")>]
    let ``RemoveValue is case-insensitive`` (key: string) =
        let args = _TestData.values
        args.RemoveValue(key)
        
        Assert.Equal(1, args.Count)
        Assert.False(args.HasValue(key))

    [<Fact>]
    let ``RemoveValue does nothing if key is missing`` () =
        let args = _TestData.values
        args.RemoveValue("missing")
        
        Assert.Equal(2, args.Count)


module Clone =
    
    [<Fact>]
    let ``clones flags and values correctly`` () =
        let clone = _TestData.mixed.Clone()

        Assert.True(clone.HasFlag("flag1"))
        Assert.True(clone.HasFlag("flag2"))
        Assert.True(clone.HasFlag("flag3"))
        Assert.Equal("hello", clone.GetValue("val1", ""))
        Assert.Equal("world", clone.GetValue("val2", ""))
    
    [<Fact>]
    let ``cloning creates a new object`` () =
        let args = _TestData.mixed

        Assert.NotSame(args.Clone(), args)
    
    [<Fact>]
    let ``modifying a clone does not modify the original`` () =
        let original = _TestData.mixed
        let clone = original.Clone()

        clone.RemoveFlag("flag1")
        clone.AddFlag("flag4")
        clone.SetValue("val1", "goodbye")

        Assert.True(original.HasFlag("flag1"))
        Assert.False(original.HasFlag("flag4"))
        Assert.Equal("hello", original.GetValue("val1", ""))


module ToDictionary =
    open System.Collections.Generic

    [<Fact>]
    let ``does nothing with empty args`` () =
        let dict = Dictionary<string, string>()
        _TestData.empty.ToDictionary(dict)

        Assert.Equal(0, dict.Count)

    [<Fact>]
    let ``converts flags and values correctly`` () =
        let dict = Dictionary<string, string>()
        _TestData.mixed.ToDictionary(dict)

        Assert.Equal(5, dict.Count)
        Assert.Equal("1", dict.["flag1"])
        Assert.Equal("1", dict.["flag2"])
        Assert.Equal("1", dict.["flag3"])
        Assert.Equal("hello", dict.["val1"])
        Assert.Equal("world", dict.["val2"])

    [<Fact>]
    let ``prefers value if the same name is used for a flag and value`` () =
        let dict = Dictionary<string, string>()
        _TestData.duplicate.ToDictionary(dict)

        Assert.Equal(1, dict.Count)
        Assert.Equal("value", dict.["duplicate"])


module ToString =

    [<Fact>]
    let ``returns empty string for empty args`` () =
        Assert.Equal("", _TestData.empty.ToString())

    [<Fact>]
    let ``converts flags and values correctly`` () =
        Assert.Equal("flag1 flag2 flag3 val1 \"hello\" val2 \"world\"", _TestData.mixed.ToString())
        // not guaranteed to be in order but works for now

    [<Fact>]
    let ``handle duplicate names in a probably pretty decent way tbh`` () =
        Assert.Equal("duplicate duplicate \"value\"", _TestData.duplicate.ToString())


module FromStringArray =

    [<Fact>]
    let ``returns empty args if input array is empty`` () =
        Assert.Equal(0, CommandLineArgs.FromStringArray('-', Array.empty).Count)

    [<Fact>]
    let ``returns empty args if no entry starts with entry char`` () =
        Assert.Equal(0, CommandLineArgs.FromStringArray('-', [| ""; "~nope"; ":fail" |]).Count)

    [<Fact>]
    let ``reads flags and values correctly`` () =
        let args = CommandLineArgs.FromStringArray('-', [| "-flag1"; "-flag2"; "-flag3"; "-val1"; "first value"; "-val2"; "second value" |])

        Assert.Equal(5, args.Count)
        Assert.True(args.HasFlag("-flag1"))
        Assert.True(args.HasFlag("-flag2"))
        Assert.True(args.HasFlag("-flag3"))
        Assert.Equal("first value", args.GetValue("-val1", ""))
        Assert.Equal("second value", args.GetValue("-val2", ""))


module ReadCefArguments =

    [<Fact>]
    let ``returns empty args if input string is empty`` () =
        Assert.Equal(0, CommandLineArgs.ReadCefArguments("").Count)

    [<Fact>]
    let ``returns empty args if input string is whitespace`` () =
        Assert.Equal(0, CommandLineArgs.ReadCefArguments(" \r\n \t").Count)

    [<Fact>]
    let ``reads values correctly`` () =
        let args = CommandLineArgs.ReadCefArguments("--first-value=10 --second-value=\"long string with spaces\"")

        Assert.Equal(2, args.Count)
        Assert.Equal("10", args.GetValue("first-value", ""))
        Assert.Equal("long string with spaces", args.GetValue("second-value", ""))

    [<Fact>]
    let ``reads flags as value keys with values of 1`` () =
        let args = CommandLineArgs.ReadCefArguments("--first-flag-as-value --second-flag-as-value")

        Assert.Equal(2, args.Count)
        Assert.Equal("1", args.GetValue("first-flag-as-value", ""))
        Assert.Equal("1", args.GetValue("second-flag-as-value", ""))

    [<Fact>]
    let ``reads complex string with whitespace correctly`` () =
        let args = CommandLineArgs.ReadCefArguments("\t--first-value=55.5\r\n--first-flag-as-value\r\n --second-value=\"long string\"\t--second-flag-as-value ")

        Assert.Equal(4, args.Count)
        Assert.Equal("55.5", args.GetValue("first-value", ""))
        Assert.Equal("long string", args.GetValue("second-value", ""))
        Assert.Equal("1", args.GetValue("first-flag-as-value", ""))
        Assert.Equal("1", args.GetValue("second-flag-as-value", ""))
