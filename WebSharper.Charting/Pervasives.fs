// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2018 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}
namespace WebSharper.Charting

open WebSharper

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

    let internal withIndex s =
        Seq.zip (Seq.initInfinite string) s

    module internal Seq =
        let headOption s =
            if Seq.isEmpty s then None
            else Some <| Seq.item 0 s

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
