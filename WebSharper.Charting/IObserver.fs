namespace WebSharper.Charting

open WebSharper

[<JavaScript>]
type IObservable<'T> =
    abstract member Subscribe : IObserver<'T> -> unit
    abstract member LastValue : 'T

and [<JavaScript>] IObserver<'T> =
    abstract member OnChange : IObservable<'T> -> unit
