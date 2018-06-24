namespace Unit.Data

open Xunit
open TweetDuck.Data
open System


module TestResult_WithValue =
    let result = Result<int>(10)

    [<Fact>]
    let ``HasValue returns true`` () =
        Assert.True(result.HasValue)

    [<Fact>]
    let ``Accessing Value returns the provided value`` () =
        Assert.Equal(10, result.Value)

    [<Fact>]
    let ``Accessing Exception throws`` () =
        Assert.Throws<InvalidOperationException>(fun () -> result.Exception |> ignore)

    [<Fact>]
    let ``Handle calls the correct callback`` () =
        let passTest = fun _ -> ()
        let failTest = fun _ -> Assert.True(false)
        result.Handle(Action<_>(passTest), Action<_>(failTest))

    [<Fact>]
    let ``Select returns another valid Result`` () =
        Assert.Equal(20, result.Select(fun x -> x * 2).Value)


module TestResult_WithException =
    let result = Result<int>(IndexOutOfRangeException("bad"))

    [<Fact>]
    let ``HasValue returns false`` () =
        Assert.False(result.HasValue)

    [<Fact>]
    let ``Accessing Value throws`` () =
        Assert.Throws<InvalidOperationException>(fun () -> result.Value |> ignore)

    [<Fact>]
    let ``Accessing Exception returns the provided exception`` () =
        Assert.IsType<IndexOutOfRangeException>(result.Exception)

    [<Fact>]
    let ``Handle calls the correct callback`` () =
        let passTest = fun _ -> ()
        let failTest = fun _ -> Assert.True(false)
        result.Handle(Action<_>(failTest), Action<_>(passTest))

    [<Fact>]
    let ``Select returns a Result with the same Exception`` () =
        Assert.Same(result.Exception, result.Select(fun x -> x * 2).Exception)
