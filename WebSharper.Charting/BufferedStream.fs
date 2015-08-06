namespace WebSharper.Charting

open System
open WebSharper

[<JavaScript>]
type private Buffer<'T> (capacity) =
    let backing = System.Collections.Generic.Queue<'T>()

    new () = Buffer(500)

    member this.Push (value : 'T) =
        backing.Enqueue value
        

        if backing.Count > capacity then
            backing.Dequeue() |> ignore
    
    member this.State = backing.ToArray()

[<JavaScript>]
type BufferedStream<'T> (capacity) =
    let buffer = Buffer<'T>(capacity)

    member val Event = Event<_>()

    interface IObservable<'T> with
        member this.Subscribe o =
            buffer.State
            |> Array.iter o.OnNext
            this.Event.Publish.Subscribe o

    member this.Trigger v =
        buffer.Push v |> ignore
        this.Event.Trigger v
