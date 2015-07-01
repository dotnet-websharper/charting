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
