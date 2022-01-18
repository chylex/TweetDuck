namespace TweetTest.Utils.IO.CombinedFileStream

open System
open System.IO
open System.Text
open TweetLib.Utils.IO
open Xunit


type internal TestData =

    static member setup(setupFunction: CombinedFileStream -> unit) =
        let writeStream = new MemoryStream()
        let readStream = new MemoryStream()

        using (new CombinedFileStream(writeStream)) (fun f ->
            setupFunction f
            writeStream.Position <- 0L
            writeStream.CopyTo(readStream)
            readStream.Position <- 0L
        )

        new CombinedFileStream(readStream)

    static member empty = TestData.setup (fun f -> f.Flush())

    static member singleFile =
        TestData.setup (fun f ->
            f.WriteString("File 1", "test file\n123")
        )

    static member singleFileWithMultiIdentifier =
        TestData.setup (fun f ->
            f.WriteString([| "File 1"; "A"; "B" |], "test file\n123")
        )

    static member singleFileStreams =
        dict [ ("singleFile", TestData.singleFile)
               ("singleFileWithMultiIdentifier", TestData.singleFileWithMultiIdentifier) ]

    static member threeFiles =
        TestData.setup (fun f ->
            f.WriteString("File 1", "Contents of\nFile 1")
            f.WriteString("File 2", "Contents of\nFile 2")
            f.WriteString("File 3", "Contents of\nFile 3")
        )


module Validation =

    [<Fact>]
    let ``an identifier containing '|' throws`` () =
        TestData.setup (fun f ->
            Assert.Throws<ArgumentException>(fun () ->
                f.WriteString("File|1", "")
            ) |> ignore
        )

    [<Fact>]
    let ``an identifier 255 bytes long does not throw`` () =
        TestData.setup (fun f ->
            f.WriteString(String.replicate 255 "a", "")
        )

    [<Fact>]
    let ``an identifier 256 bytes long throws`` () =
        TestData.setup (fun f ->
            Assert.Throws<ArgumentOutOfRangeException>(fun () ->
                f.WriteString(String.replicate 256 "a", "")
            ) |> ignore
        )


module ReadFile =

    [<Fact>]
    let ``reading empty file returns null`` () =
        using TestData.empty (fun f ->
            Assert.Null(f.ReadFile())
        )

    [<Theory>]
    [<InlineData("singleFile")>]
    [<InlineData("singleFileWithMultiIdentifier")>]
    let ``reading first file from single file stream returns an entry`` (stream: string) =
        using TestData.singleFileStreams.[stream] (fun f ->
            Assert.NotNull(f.ReadFile())
        )

    [<Theory>]
    [<InlineData("singleFile")>]
    [<InlineData("singleFileWithMultiIdentifier")>]
    let ``reading second file from single file stream returns null`` (stream: string) =
        using TestData.singleFileStreams.[stream] (fun f ->
            f.ReadFile() |> ignore
            Assert.Null(f.ReadFile())
        )

    [<Fact>]
    let ``reading first three files from a three file stream returns an entry`` () =
        using TestData.threeFiles (fun f ->
            Assert.NotNull(f.ReadFile())
            Assert.NotNull(f.ReadFile())
            Assert.NotNull(f.ReadFile())
        )

    [<Fact>]
    let ``reading more files beyond a three file stream returns null`` () =
        using TestData.threeFiles (fun f ->
            f.ReadFile() |> ignore
            f.ReadFile() |> ignore
            f.ReadFile() |> ignore
            Assert.Null(f.ReadFile())
            Assert.Null(f.ReadFile())
        )


module SkipFile =

    [<Fact>]
    let ``skipping empty file returns null`` () =
        using TestData.empty (fun f ->
            Assert.Null(f.SkipFile())
        )

    [<Fact>]
    let ``skipping first file from single file stream returns an identifier`` () =
        using TestData.singleFile (fun f ->
            Assert.Equal("File 1", f.SkipFile())
        )

    [<Fact>]
    let ``skipping first file from single file stream returns the first part of a multi identifier`` () =
        using TestData.singleFileWithMultiIdentifier (fun f ->
            Assert.Equal("File 1", f.SkipFile())
        )

    [<Fact>]
    let ``skipping second file from single file stream returns null`` () =
        using TestData.singleFile (fun f ->
            f.SkipFile() |> ignore
            Assert.Null(f.SkipFile())
        )

    [<Fact>]
    let ``skipping first three files from a three file stream returns their identifiers`` () =
        using TestData.threeFiles (fun f ->
            Assert.Equal("File 1", f.SkipFile())
            Assert.Equal("File 2", f.SkipFile())
            Assert.Equal("File 3", f.SkipFile())
        )

    [<Fact>]
    let ``skipping more files beyond a three file stream returns null`` () =
        using TestData.threeFiles (fun f ->
            f.SkipFile() |> ignore
            f.SkipFile() |> ignore
            f.SkipFile() |> ignore
            Assert.Null(f.SkipFile())
            Assert.Null(f.SkipFile())
        )


module Entry =

    module KeyName =

        [<Fact>]
        let ``simple identifier returns itself`` () =
            using TestData.singleFile (fun f ->
                Assert.Equal("File 1", f.ReadFile().KeyName)
            )

        [<Fact>]
        let ``multi identifier returns the first part`` () =
            using TestData.singleFileWithMultiIdentifier (fun f ->
                Assert.Equal("File 1", f.ReadFile().KeyName)
            )

    module KeyValue =

        [<Fact>]
        let ``simple identifier returns an empty array`` () =
            using TestData.singleFile (fun f ->
                Assert.Equal<string[]>(Array.empty, f.ReadFile().KeyValue)
            )

        [<Fact>]
        let ``multi identifier returns all but the first part`` () =
            using TestData.singleFileWithMultiIdentifier (fun f ->
                Assert.Equal<string[]>([| "A"; "B" |], f.ReadFile().KeyValue)
            )

    module Contents =

        [<Theory>]
        [<InlineData("singleFile")>]
        [<InlineData("singleFileWithMultiIdentifier")>]
        let ``contents of single file stream with are correct`` (stream: string) =
            using TestData.singleFileStreams.[stream] (fun f ->
                Assert.Equal("test file\n123", Encoding.UTF8.GetString(f.ReadFile().Contents))
            )

        [<Fact>]
        let ``contents of three file stream are correct`` () =
            using TestData.threeFiles (fun f ->
                Assert.Equal("Contents of\nFile 1", Encoding.UTF8.GetString(f.ReadFile().Contents))
                Assert.Equal("Contents of\nFile 2", Encoding.UTF8.GetString(f.ReadFile().Contents))
                Assert.Equal("Contents of\nFile 3", Encoding.UTF8.GetString(f.ReadFile().Contents))
            )
