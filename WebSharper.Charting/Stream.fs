namespace WebSharper.Charting

open System
open WebSharper

[<JavaScript>]
type BufferedStream<'T> private (bc) =
    let buffer = Buffer<'T>(bc)

    member val Event = Event<_>()
    member val Window = bc

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

    static member FromList (source : 'T list) =
        let stream = BufferedStream(List.length source)

        source
        |> List.iter (fun value ->
            stream.Trigger value
        )

        stream
