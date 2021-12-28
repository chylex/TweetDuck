namespace TweetTest.Utils.Data.Result

open System
open TweetLib.Utils.Data
open Xunit


type internal TestData =

    static member value = Result<int>(10)
    static member ex = Result<int>(NullReferenceException("Test Exception"))


module HasValue =

    [<Fact>]
    let ``returns true if value is present`` () =
        Assert.True(TestData.value.HasValue)

    [<Fact>]
    let ``returns false if value is not present`` () =
        Assert.False(TestData.ex.HasValue)


module Value =

    [<Fact>]
    let ``returns value if value is present`` () =
        Assert.Equal(10, TestData.value.Value)

    [<Fact>]
    let ``throws if value is not present`` () =
        Assert.Throws<InvalidOperationException>(fun () -> TestData.ex.Value |> ignore)


module Exception =

    [<Fact>]
    let ``throws if value is present`` () =
        Assert.Throws<InvalidOperationException>(fun () -> TestData.value.Exception |> ignore)

    [<Fact>]
    let ``returns exception if value is not present`` () =
        Assert.IsType<NullReferenceException>(TestData.ex.Exception)


module Handle =
    let passTest = Action<_>(fun _ -> Assert.True(true))
    let failTest = Action<_>(fun _ -> Assert.True(false))

    [<Fact>]
    let ``executes the correct action if value is present`` () =
        TestData.value.Handle(passTest, failTest)

    [<Fact>]
    let ``executes the correct action if exception is present`` () = TestData.ex.Handle(failTest, passTest)


module Select =

    [<Fact>]
    let ``returns successful result with mapped value if value is present`` () =
        let newResult = TestData.value.Select(fun x -> x * 2)
        Assert.Equal(20, newResult.Value)

    [<Fact>]
    let ``returns typed result with same exception if value is not present`` () =
        let oldResult = TestData.ex
        let newResult = oldResult.Select(fun x -> x * 2)
        Assert.Same(oldResult.Exception, newResult.Exception)
