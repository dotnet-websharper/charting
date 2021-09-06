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
open System


[<AutoOpen; JavaScript>]
module Reactive =

    let New fn =
        { new IObservable<'T> with
            member this.Subscribe o = fn o
        }

    let NewObserver onNext onComplete =
        { new IObserver<'T> with
            member this.OnNext t = onNext t
            member this.OnCompleted() = onComplete()
            member this.OnError err = ()
        }

    let NewDisposable d =
        { new IDisposable with 
            member this.Dispose() = d () 
        }

    let Return<'T> (x: 'T) : IObservable<'T> =
        let f (o : IObserver<'T>) =
            o.OnNext x
            o.OnCompleted ()
            NewDisposable ignore
        New f

    let Select (io: IObservable<'T>) fn = 
        New <| fun o ->
            io.Subscribe(fun v -> o.OnNext (fn v))

    let CombineLast (io1: IObservable<'T>) (io2: IObservable<'U>)
        (f: 'T -> 'U -> 'S) : IObservable<'S> =
        New <| fun o ->
            let lv1s = System.Collections.Generic.Queue<'T>()
            let lv2s = System.Collections.Generic.Queue<'U>()
            let update () =
                if lv1s.Count > 0 && lv2s.Count > 0 then
                    let v1 = lv1s.Dequeue()
                    let v2 = lv2s.Dequeue()
                    o.OnNext (f v1 v2)
            let o1 =
                let onNext x =
                    lv1s.Enqueue(x)
                    update ()
                NewObserver onNext ignore
            let o2 =
                let onNext y =
                    lv2s.Enqueue(y)
                    update ()
                NewObserver onNext ignore
            let d1 = io1.Subscribe o1
            let d2 = io2.Subscribe o2
            NewDisposable (fun () -> d1.Dispose() ; d2.Dispose())

    let Aggregate (io: IObservable<'T>) (seed: 'S) (acc: 'S -> 'T -> 'S) =
        New (fun o ->
            let state = ref seed
            io.Subscribe(fun value ->
                state := acc state.Value value
                o.OnNext(state.Value)))
