namespace WebSharper.Charting

open WebSharper
open IntelliFactory.Reactive

[<AutoOpen; JavaScript>]
module Pervasives =
    type Size = Size of width : int * height : int

    [<RequireQualifiedAccess>]
    type Color =
        | Rgba  of red : int * green : int * blue : int * alpha : float
        | Hex  of string
        | Name of string

        with
         override x.ToString () =
            match x with
            | Rgba(r, g, b, a) -> sprintf "rgba(%d, %d, %d, %f)" r g b a
            | Hex h -> h
            | Name n -> n

    let internal streamWithLabel stream =
        Reactive.Select
        <| Reactive.Aggregate stream (0, 0.0) (fun (s, _) c -> (s + 1, c)) 
        <| fun (a, b) -> (string a, b)