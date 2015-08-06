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

    module internal Seq =
        let headOption s =
            if Seq.isEmpty s then None
            else Some <| Seq.nth 0 s

    module internal Reactive =
        let rec private sequence acc = function
            | [] -> acc
            | x :: xs ->
                sequence (Reactive.CombineLast acc x <| fun o c ->
                    Seq.append o <| Seq.singleton c) xs

        let SequenceOnlyNew streams =
            
            match Seq.toList streams with
            | [] -> Reactive.Return Seq.empty
            | x :: xs ->
                sequence (Reactive.Select x Seq.singleton) xs
