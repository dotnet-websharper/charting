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

[<AutoOpen>]
module Pervasives = begin
    type Size = | Size of width: int * height: int
    
    [<RequireQualifiedAccessAttribute>]
    type Color =
        | Rgba of red: int * green: int * blue: int * alpha: float
        | Hex  of string
        | Name of string
        
        with override ToString : unit -> string

    module internal Seq = begin
        val headOption : seq<'a> -> option<'a>
    end

    module internal Reactive = begin
        val SequenceOnlyNew : seq<#System.IObservable<'a>> -> System.IObservable<seq<'a>>
    end

    val internal streamWithLabel : System.IObservable<float> -> System.IObservable<string * float>
    val internal withIndex : seq<'T> -> seq<string * 'T>
end
