namespace TweetTest.Data.TwoKeyDictionary

open Xunit
open TweetDuck.Data
open System.Collections.Generic


type _TestData =

    static member empty
        with get() = TwoKeyDictionary<string, int, float>()

    static member uniquevals
        with get() =
            let dict = TwoKeyDictionary<string, int, float>()
            dict.Add("first", 1, 10.0)
            dict.Add("first", 2, 20.0)
            dict.Add("first", 3, 30.0)
            dict.Add("second", 1, 100.0)
            dict.Add("second", 2, 200.0)
            dict.Add("third", 1, 1000.0)
            dict

    static member duplicatevals
        with get() =
            let dict = TwoKeyDictionary<string, int, float>()
            dict.Add("first", 1, 10.0)
            dict.Add("first", 2, 20.0)
            dict.Add("first", 3, 30.0)
            dict.Add("second", 1, 10.0)
            dict.Add("second", 2, 20.0)
            dict.Add("third", 1, 10.0)
            dict


module Indexer =
    
    [<Theory>]
    [<InlineData("first", 3, 30.0)>]
    [<InlineData("second", 2, 200.0)>]
    [<InlineData("third", 1, 1000.0)>]
    let ``get returns correct value`` (outerKey: string, innerKey: int, value: float) =
        Assert.Equal(value, _TestData.uniquevals.[outerKey, innerKey])

    [<Fact>]
    let ``get throws if outer key is missing`` () =
        Assert.Throws<KeyNotFoundException>(fun () -> _TestData.uniquevals.["missing", 1] |> ignore)

    [<Fact>]
    let ``get throws if inner key is missing`` () =
        Assert.Throws<KeyNotFoundException>(fun () -> _TestData.uniquevals.["first", 0] |> ignore)

    [<Fact>]
    let ``set correctly updates value`` () =
        let copy = _TestData.uniquevals
        copy.["first", 1] <- 50.0

        Assert.Equal(50.0, copy.["first", 1])

    [<Fact>]
    let ``set creates new inner key`` () =
        let copy = _TestData.uniquevals
        copy.["second", 3] <- 300.0

        Assert.Equal(300.0, copy.["second", 3])

    [<Fact>]
    let ``set creates new outer key`` () =
        let copy = _TestData.uniquevals
        copy.["fourth", 1] <- 10000.0

        Assert.Equal(10000.0, copy.["fourth", 1])


module InnerValues =
    open System.Linq

    [<Fact>]
    let ``returns empty collection for empty dictionary`` () =
        Assert.Equal<IEnumerable<float>>(Enumerable.Empty<float>(), _TestData.empty.InnerValues)

    [<Fact>]
    let ``returns all values for dictionary with unique values`` () =
        Assert.Equal([ 10.0; 20.0; 30.0; 100.0; 200.0; 1000.0 ], _TestData.uniquevals.InnerValues)

    [<Fact>]
    let ``returns all values for dictionary with duplicated values`` () =
        Assert.Equal([ 10.0; 20.0; 30.0; 10.0; 20.0; 10.0 ], _TestData.duplicatevals.InnerValues)


module TryGetValue =

    [<Fact>]
    let ``returns true and correct value for existing key`` () =
        let (success, result) = _TestData.uniquevals.TryGetValue("first", 3)

        Assert.True(success)
        Assert.Equal(30.0, result)

    [<Fact>]
    let ``returns false for missing inner key`` () =
        Assert.False(_TestData.uniquevals.TryGetValue("first", 0, ref 0.0))

    [<Fact>]
    let ``returns false for missing outer key`` () =
        Assert.False(_TestData.uniquevals.TryGetValue("missing", 0, ref 0.0))


module Add =
    open System

    [<Fact>]
    let ``creates new inner key`` () =
        let copy = _TestData.uniquevals
        copy.Add("first", 4, 40.0)

        Assert.Equal(40.0, copy.["first", 4])

    [<Fact>]
    let ``creates new outer key`` () =
        let copy = _TestData.uniquevals
        copy.Add("fourth", 1, 10000.0)

        Assert.Equal(10000.0, copy.["fourth", 1])

    [<Fact>]
    let ``throw on duplicate key`` () =
        Assert.Throws<ArgumentException>(fun () -> _TestData.uniquevals.Add("first", 2, 25.0))


module Contains =

    [<Theory>]
    [<InlineData("first")>]
    [<InlineData("second")>]
    [<InlineData("third")>]
    let ``returns true if outer key exists`` (outerKey: string) =
        Assert.True(_TestData.uniquevals.Contains(outerKey))
        
    [<Theory>]
    [<InlineData(1)>]
    [<InlineData(2)>]
    [<InlineData(3)>]
    let ``returns true if inner key exists`` (innerKey: int) =
        Assert.True(_TestData.uniquevals.Contains("first", innerKey))

    [<Fact>]
    let ``returns false if outer key does not exist`` () =
        Assert.False(_TestData.uniquevals.Contains("missing"))

    [<Fact>]
    let ``returns false if inner key does not exist`` () =
        Assert.False(_TestData.uniquevals.Contains("first", 0))


module Count =

    [<Fact>]
    let ``counts all values for dictionary with unique values`` () =
        Assert.Equal(6, _TestData.uniquevals.Count())

    [<Fact>]
    let ``counts all values for dictionary with duplicated values`` () =
        Assert.Equal(6, _TestData.duplicatevals.Count())
        
    [<Theory>]
    [<InlineData("first", 3)>]
    [<InlineData("second", 2)>]
    [<InlineData("third", 1)>]
    let ``counts all values for specified key`` (outerKey: string, expectedCount: int) =
        Assert.Equal(expectedCount, _TestData.uniquevals.Count(outerKey))

    [<Fact>]
    let ``throws on missing key`` () =
        Assert.Throws<KeyNotFoundException>(fun () -> _TestData.uniquevals.Count("missing") |> ignore)


module Clear =

    [<Fact>]
    let ``clears all values for all keys`` () =
        let copy = _TestData.uniquevals
        copy.Clear()

        Assert.Equal(0, copy.Count())

    [<Fact>]
    let ``clears all values for specified key`` () =
        let copy = _TestData.uniquevals
        copy.Clear("first")
        
        Assert.True(copy.Contains("first"))
        Assert.Equal(0, copy.Count("first"))
        Assert.Equal(3, copy.Count())

    [<Fact>]
    let ``throws on missing key`` () =
        Assert.Throws<KeyNotFoundException>(fun () -> _TestData.uniquevals.Clear("missing") |> ignore)


module Remove =

    [<Fact>]
    let ``removes value by key pair`` () =
        let copy = _TestData.uniquevals
        Assert.True(copy.Remove("first", 3))

        Assert.False(copy.Contains("first", 3))
        Assert.Equal(5, copy.Count())

    [<Fact>]
    let ``removes inner key and its values`` () =
        let copy = _TestData.uniquevals
        Assert.True(copy.Remove("first"))

        Assert.False(copy.Contains("first"))
        Assert.Equal(3, copy.Count())

    [<Fact>]
    let ``removing all inner keys deletes the outer key`` () =
        let copy = _TestData.uniquevals
        Assert.True(copy.Remove("first", 1))
        Assert.True(copy.Remove("first", 2))
        Assert.True(copy.Remove("first", 3))
        Assert.False(copy.Contains("first"))

    [<Fact>]
    let ``returns false on missing inner key`` () =
        Assert.False(_TestData.uniquevals.Remove("first", 0))

    [<Fact>]
    let ``returns false on missing outer key`` () =
        Assert.False(_TestData.uniquevals.Remove("missing"))
