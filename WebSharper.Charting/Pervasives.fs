namespace WebSharper.Charting

open WebSharper

[<AutoOpen; JavaScript>]
module Pervasives =
    type Size = Size of width : int * height : int

