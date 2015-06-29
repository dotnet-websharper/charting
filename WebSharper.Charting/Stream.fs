namespace WebSharper.Charting

open WebSharper

[<JavaScript>]
type Stream<'T> private (bc) =
    let observers : IObserver<'T> array = Array.empty
    let buffer = Buffer<'T>(bc)
    
    let mutable lastValue : 'T option = None

    interface IObservable<'T> with
        member this.Subscribe (observer : IObserver<'T>) =
            observers
            |> Array.push observer

            buffer.State
            |> Array.iter (fun value ->
                lastValue <- Some value

                this.NotifyObservers false
            )

        member this.LastValue = lastValue.Value
    
    member private this.NotifyObservers o =
        observers
        |> Array.iter (fun observer ->
            observer.OnChange (this, o)
        )

    member this.Push (value : 'T) =
        lastValue <- Some value
        
        buffer.Push value
        |> this.NotifyObservers

    static member FromList (source : 'T list) =
        let stream = Stream(List.length source)

        source
        |> List.iter (fun value ->
            stream.Push value
        )

        stream
