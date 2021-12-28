namespace TweetTest.Core.Data.CommandLineArgs

open TweetLib.Utils.Collections
open Xunit


type internal TestData =

    static member empty = CommandLineArgs()

    static member flags =
        let args = CommandLineArgs()
        args.AddFlag("flag1")
        args.AddFlag("flag2")
        args.AddFlag("flag3")
        args

    static member values =
        let args = CommandLineArgs()
        args.SetValue("val1", "hello")
        args.SetValue("val2", "world")
        args

    static member mixed =
        let args = CommandLineArgs()
        args.AddFlag("flag1")
        args.AddFlag("flag2")
        args.AddFlag("flag3")
        args.SetValue("val1", "hello")
        args.SetValue("val2", "world")
        args

    static member duplicate =
        let args = CommandLineArgs()
        args.AddFlag("duplicate")
        args.SetValue("duplicate", "value")
        args


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
        Assert.Equal("first value", args.GetValue("-val1"))
        Assert.Equal("second value", args.GetValue("-val2"))


module Count =

    [<Fact>]
    let ``counts nothing correctly`` () =
        Assert.Equal(0, TestData.empty.Count)

    [<Fact>]
    let ``counts flags correctly`` () =
        Assert.Equal(3, TestData.flags.Count)

    [<Fact>]
    let ``counts values correctly`` () =
        Assert.Equal(2, TestData.values.Count)

    [<Fact>]
    let ``counts mixed flags and values correctly`` () =
        Assert.Equal(5, TestData.mixed.Count)


module Flags =

    [<Theory>]
    [<InlineData("flag1")>]
    [<InlineData("flag2")>]
    [<InlineData("flag3")>]
    let ``HasFlag returns false if flag is missing`` (flag: string) =
        Assert.False(TestData.empty.HasFlag(flag))
        Assert.False(TestData.values.HasFlag(flag))

    [<Theory>]
    [<InlineData("val1")>]
    [<InlineData("val2")>]
    let ``HasFlag returns false if the name only specifies a key`` (flag: string) =
        Assert.False(TestData.values.HasFlag(flag))

    [<Fact>]
    let ``HasFlag returns true if the name specifies both a flag and a value key`` () =
        Assert.True(TestData.duplicate.HasFlag("duplicate"))

    [<Theory>]
    [<InlineData("flag1")>]
    [<InlineData("flag2")>]
    [<InlineData("flag3")>]
    let ``HasFlag returns true if flag is present`` (flag: string) =
        Assert.True(TestData.flags.HasFlag(flag))

    [<Theory>]
    [<InlineData("FLAG1")>]
    [<InlineData("FlAg1")>]
    let ``HasFlag is case-insensitive`` (flag: string) =
        Assert.True(TestData.flags.HasFlag(flag))

    [<Fact>]
    let ``AddFlag adds new flag`` () =
        let args = TestData.flags
        args.AddFlag("flag4")

        Assert.Equal(4, args.Count)
        Assert.True(args.HasFlag("flag4"))

    [<Fact>]
    let ``AddFlag does nothing if flag is already present`` () =
        let args = TestData.flags
        args.AddFlag("flag1")

        Assert.Equal(3, args.Count)

    [<Theory>]
    [<InlineData("flag1")>]
    [<InlineData("flag2")>]
    [<InlineData("flag3")>]
    let ``RemoveFlag removes existing flag`` (flag: string) =
        let args = TestData.flags
        args.RemoveFlag(flag)

        Assert.Equal(2, args.Count)
        Assert.False(args.HasFlag(flag))

    [<Theory>]
    [<InlineData("FLAG1")>]
    [<InlineData("FlAg1")>]
    let ``RemoveFlag is case-insensitive`` (flag: string) =
        let args = TestData.flags
        args.RemoveFlag(flag)

        Assert.Equal(2, args.Count)
        Assert.False(args.HasFlag(flag))

    [<Fact>]
    let ``RemoveFlag does nothing if flag is missing`` () =
        let args = TestData.flags
        args.RemoveFlag("missing")

        Assert.Equal(3, args.Count)


module Values =

    [<Fact>]
    let ``GetValue returns null if key is missing`` () =
        Assert.Null(TestData.values.GetValue("missing"))

    [<Theory>]
    [<InlineData("flag1")>]
    [<InlineData("flag2")>]
    [<InlineData("flag3")>]
    let ``GetValue returns null if the name specifies a flag`` (key: string) =
        Assert.Null(TestData.flags.GetValue(key))

    [<Fact>]
    let ``GetValue returns correct value if the name specifies both a flag and a value key`` () =
        Assert.NotNull(TestData.duplicate.GetValue("duplicate"))

    [<Theory>]
    [<InlineData("val1", "hello")>]
    [<InlineData("val2", "world")>]
    let ``GetValue returns correct value if key is present`` (key: string, expectedValue: string) =
        Assert.Equal(expectedValue, TestData.values.GetValue(key))

    [<Theory>]
    [<InlineData("VAL1", "hello")>]
    [<InlineData("VaL1", "hello")>]
    let ``GetValue is case-insensitive`` (key: string, expectedValue: string) =
        Assert.Equal(expectedValue, TestData.values.GetValue(key))

    [<Fact>]
    let ``SetValue adds new value`` () =
        let args = TestData.values
        args.SetValue("val3", "this is nice")

        Assert.Equal(3, args.Count)
        Assert.Equal("this is nice", args.GetValue("val3"))

    [<Fact>]
    let ``SetValue replaces existing value`` () =
        let args = TestData.values
        args.SetValue("val2", "mom")

        Assert.Equal(2, args.Count)
        Assert.Equal("mom", args.GetValue("val2"))

    [<Theory>]
    [<InlineData("val1")>]
    [<InlineData("val2")>]
    let ``RemoveValue removes existing key`` (key: string) =
        let args = TestData.values
        args.RemoveValue(key)

        Assert.Equal(1, args.Count)
        Assert.Null(args.GetValue(key))

    [<Theory>]
    [<InlineData("VAL1")>]
    [<InlineData("VaL1")>]
    let ``RemoveValue is case-insensitive`` (key: string) =
        let args = TestData.values
        args.RemoveValue(key)

        Assert.Equal(1, args.Count)
        Assert.Null(args.GetValue(key))

    [<Fact>]
    let ``RemoveValue does nothing if key is missing`` () =
        let args = TestData.values
        args.RemoveValue("missing")

        Assert.Equal(2, args.Count)


module Clone =

    [<Fact>]
    let ``clones flags and values correctly`` () =
        let clone = TestData.mixed.Clone()

        Assert.True(clone.HasFlag("flag1"))
        Assert.True(clone.HasFlag("flag2"))
        Assert.True(clone.HasFlag("flag3"))
        Assert.Equal("hello", clone.GetValue("val1"))
        Assert.Equal("world", clone.GetValue("val2"))

    [<Fact>]
    let ``cloning creates a new object`` () =
        let args = TestData.mixed

        Assert.NotSame(args.Clone(), args)

    [<Fact>]
    let ``modifying a clone does not modify the original`` () =
        let original = TestData.mixed
        let clone = original.Clone()

        clone.RemoveFlag("flag1")
        clone.AddFlag("flag4")
        clone.SetValue("val1", "goodbye")

        Assert.True(original.HasFlag("flag1"))
        Assert.False(original.HasFlag("flag4"))
        Assert.Equal("hello", original.GetValue("val1"))


module ToDictionary =
    open System.Collections.Generic

    [<Fact>]
    let ``does nothing with empty args`` () =
        let dict = Dictionary<string, string>()
        TestData.empty.ToDictionary(dict)

        Assert.Equal(0, dict.Count)

    [<Fact>]
    let ``converts flags and values correctly`` () =
        let dict = Dictionary<string, string>()
        TestData.mixed.ToDictionary(dict)

        Assert.Equal(5, dict.Count)
        Assert.Equal("1", dict.["flag1"])
        Assert.Equal("1", dict.["flag2"])
        Assert.Equal("1", dict.["flag3"])
        Assert.Equal("hello", dict.["val1"])
        Assert.Equal("world", dict.["val2"])

    [<Fact>]
    let ``prefers value if the same name is used for a flag and value`` () =
        let dict = Dictionary<string, string>()
        TestData.duplicate.ToDictionary(dict)

        Assert.Equal(1, dict.Count)
        Assert.Equal("value", dict.["duplicate"])


module ToString =

    [<Fact>]
    let ``returns empty string for empty args`` () =
        Assert.Equal("", TestData.empty.ToString())

    [<Fact>]
    let ``converts flags and values correctly`` () =
        // not guaranteed to be in order but works for now
        Assert.Equal("flag1 flag2 flag3 val1 \"hello\" val2 \"world\"", TestData.mixed.ToString())

    [<Fact>]
    let ``handle duplicate names in a probably pretty decent way tbh`` () =
        Assert.Equal("duplicate duplicate \"value\"", TestData.duplicate.ToString())
