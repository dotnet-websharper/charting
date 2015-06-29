namespace WebSharper.Charting

open WebSharper

[<JavaScript>]
type Stream<'T> () =
    let observers : IObserver<'T> array = Array.empty
    let buffer = Buffer<'T>()
    
    let mutable lastValue : 'T option = None

    interface IObservable<'T> with
        member this.Subscribe (observer : IObserver<'T>) =
            observers
            |> Array.push observer

            buffer.State
            |> Array.iter (fun value ->
                lastValue <- Some value

                this.NotifyObservers ()
            )

        member this.LastValue = lastValue.Value
    
    member private this.NotifyObservers () =
        observers
        |> Array.iter (fun observer ->
            observer.OnChange this
        )

    member this.Push (value : 'T) =
        lastValue <- Some value
        
        this.NotifyObservers ()
        buffer.Push value

    static member FromList (source : 'T list) =
        let stream = Stream()

        source
        |> List.iter (fun value ->
            stream.Push value
        )

        stream
