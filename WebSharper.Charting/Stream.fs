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

open System
open WebSharper

[<JavaScript>]
type BufferedStream<'T> private (?bc) =
    let buffer = 
        match bc with
        | Some bc -> Buffer<'T>(bc)
        | None -> Buffer<'T>()

    member val Event = Event<_>()

    interface IObservable<'T> with
        [<JavaScript>]
        member this.Subscribe o =
            buffer.State
            |> Array.iter o.OnNext
            this.Event.Publish.Subscribe o
    
    member this.Trigger v =
        buffer.Push v |> ignore
        this.Event.Trigger v

    static member New(capacity : int) : BufferedStream<_> =
        BufferedStream(capacity)

    static member New() : BufferedStream<_> =
        BufferedStream()

    static member FromList (source : 'T list) =
        let stream = BufferedStream(List.length source)

        source
        |> List.iter (fun value ->
            stream.Trigger value
        )

        stream
