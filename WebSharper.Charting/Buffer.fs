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
open WebSharper.JavaScript

[<JavaScript>]
module internal Array =
    
    [<Inline "$1.push($0)">]
    let push (_ : 'T) (_ : 'T array) = ()

    [<Inline "$0.shift()">]
    let shift (_ : 'T array) = ()

[<JavaScript>]
type private Buffer<'T> (?capacity) =
    let capacity = defaultArg capacity 500
    let backing : 'T array = Array.empty

    member this.Push (value : 'T) =
        backing
        |> Array.push value

        if backing.Length > capacity then
            Array.shift backing
            true
        else
            false
    
    member this.State = backing
