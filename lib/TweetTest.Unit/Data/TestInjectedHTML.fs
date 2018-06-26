namespace Unit.Data.InjectedHTML

open Xunit
open TweetDuck.Data


module Inject =
    let before = InjectedHTML.Position.Before
    let after = InjectedHTML.Position.After
    
    [<Fact>]
    let ``injecting string before searched string works`` () =
        Assert.Equal("<p>source[left]<br>code</p>", InjectedHTML(before, "<br>", "[left]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``injecting string after searched string works`` () =
        Assert.Equal("<p>source<br>[right]code</p>", InjectedHTML(after, "<br>", "[right]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``injecting string at the beginning works`` () =
        Assert.Equal("[start]<p>source<br>code</p>", InjectedHTML(before, "<p>", "[start]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``injecting string at the end works`` () =
        Assert.Equal("<p>source<br>code</p>[end]", InjectedHTML(after, "</p>", "[end]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``injection only triggers for first occurrence of searched string`` () =
        Assert.Equal("<p>source[left]<br>code</p><br>", InjectedHTML(before, "<br>", "[left]").InjectInto("<p>source<br>code</p><br>"))
        Assert.Equal("<p>source<br>[right]code</p><br>", InjectedHTML(after, "<br>", "[right]").InjectInto("<p>source<br>code</p><br>"))

    [<Fact>]
    let ``empty searched string injects at the beginning`` () =
        Assert.Equal("[start]<p>source<br>code</p>", InjectedHTML(before, "", "[start]").InjectInto("<p>source<br>code</p>"))
        Assert.Equal("[start]<p>source<br>code</p>", InjectedHTML(after, "", "[start]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``injecting empty string does not modify source`` () =
        Assert.Equal("<p>source<br>code</p>", InjectedHTML(before, "<br>", "").InjectInto("<p>source<br>code</p>"))
        Assert.Equal("<p>source<br>code</p>", InjectedHTML(after, "<br>", "").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``failed match does not modify source`` () =
        Assert.Equal("<p>source<br>code</p>", InjectedHTML(before, "<wrong>", "[left]").InjectInto("<p>source<br>code</p>"))
        Assert.Equal("<p>source<br>code</p>", InjectedHTML(after, "<wrong>", "[right]").InjectInto("<p>source<br>code</p>"))

    [<Fact>]
    let ``invalid position does not modify source`` () =
        Assert.Equal("<p>source<br>code</p>", InjectedHTML(enum<_>(1000), "<br>", "[somewhere]").InjectInto("<p>source<br>code</p>"))
