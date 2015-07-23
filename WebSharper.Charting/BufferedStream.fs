namespace WebSharper.Charting

open System
open WebSharper

[<JavaScript>]
module internal Array =
    
    [<Inline "$1.push($0)">]
    let push (_ : 'T) (_ : 'T array) = ()

    [<Inline "$0.shift()">]
    let shift (_ : 'T array) = ()

[<JavaScript>]
type private Buffer<'T> (capacity) =
    let backing : 'T array = Array.empty

    new () = Buffer(500)

    member this.Push (value : 'T) =
        backing
        |> Array.push value

        if backing.Length > capacity then
            Array.shift backing
            true
        else
            false
    
    member this.State = backing

[<JavaScript>]
type BufferedStream<'T> (capacity) =
    let buffer = Buffer<'T>(capacity)

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

[<JavaScript>]
type BufferedStream =
    static member FromList (source : 'T list) =
        let stream = BufferedStream(List.length source)

        source
        |> List.iter (fun value ->
            stream.Trigger value)

        stream
