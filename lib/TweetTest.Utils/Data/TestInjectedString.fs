namespace TweetTest.Utils.Data.InjectedString

open TweetLib.Utils.Data
open Xunit


module InjectInto =
    let before = InjectedString.Position.Before
    let after = InjectedString.Position.After

    [<Fact>]
    let ``injecting string before searched string works`` () =
        Assert.Equal("<p>source[left]<br>code</p>", InjectedString(before, "<br>", "[left]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``injecting string after searched string works`` () =
        Assert.Equal("<p>source<br>[right]code</p>", InjectedString(after, "<br>", "[right]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``injecting string at the beginning works`` () =
        Assert.Equal("[start]<p>source<br>code</p>", InjectedString(before, "<p>", "[start]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``injecting string at the end works`` () =
        Assert.Equal("<p>source<br>code</p>[end]", InjectedString(after, "</p>", "[end]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``injection only triggers for first occurrence of searched string`` () =
        Assert.Equal("<p>source[left]<br>code</p><br>", InjectedString(before, "<br>", "[left]").InjectInto("<p>source<br>code</p><br>"))
        Assert.Equal("<p>source<br>[right]code</p><br>", InjectedString(after, "<br>", "[right]").InjectInto("<p>source<br>code</p><br>"))

    [<Fact>]
    let ``empty searched string injects at the beginning`` () =
        Assert.Equal("[start]<p>source<br>code</p>", InjectedString(before, "", "[start]").InjectInto("<p>source<br>code</p>"))
        Assert.Equal("[start]<p>source<br>code</p>", InjectedString(after, "", "[start]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``injecting empty string does not modify source`` () =
        Assert.Equal("<p>source<br>code</p>", InjectedString(before, "<br>", "").InjectInto("<p>source<br>code</p>"))
        Assert.Equal("<p>source<br>code</p>", InjectedString(after, "<br>", "").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``failed match does not modify source`` () =
        Assert.Equal("<p>source<br>code</p>", InjectedString(before, "<wrong>", "[left]").InjectInto("<p>source<br>code</p>"))
        Assert.Equal("<p>source<br>code</p>", InjectedString(after, "<wrong>", "[right]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``invalid position does not modify source`` () =
        Assert.Equal("<p>source<br>code</p>", InjectedString(enum<_>(1000), "<br>", "[somewhere]").InjectInto("<p>source<br>code</p>"))
