namespace TweetTest.Utils.Dialogs.FileDialogFilter

open TweetLib.Utils.Dialogs
open Xunit


type internal TestData =
    
    static member noExtensions = FileDialogFilter("Any Filter")
    static member oneExtension = FileDialogFilter("Single Filter", ".1")
    static member twoExtensions = FileDialogFilter("Double Filter", ".1", ".2")


module Pattern =
    
    [<Fact>]
    let ``no extension assumes any files`` () =
        Assert.Equal("*.*", TestData.noExtensions.Pattern)
        
    [<Fact>]
    let ``one extension prepends with asterisk`` () =
        Assert.Equal("*.1", TestData.oneExtension.Pattern)
        
    [<Fact>]
    let ``multiple extensions are separated by semicolons`` () =
        Assert.Equal("*.1;*.2", TestData.twoExtensions.Pattern)


module FullName =
    
    [<Fact>]
    let ``no extension keeps original name`` () =
        Assert.Equal("Any Filter", TestData.noExtensions.FullName)
        
    [<Fact>]
    let ``one extension appends pattern in parentheses`` () =
        Assert.Equal("Single Filter (*.1)", TestData.oneExtension.FullName)
        
    [<Fact>]
    let ``multiple extensions in pattern are separated by semicolons`` () =
        Assert.Equal("Double Filter (*.1;*.2)", TestData.twoExtensions.FullName)
