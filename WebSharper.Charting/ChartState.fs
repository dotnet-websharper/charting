namespace WebSharper.Charting

open WebSharper
open WebSharper.Html.Client

[<JavaScript>]
type ChartState<'T> =
    {
        Renderer : IRenderer<'T>
        Dataset  : System.IObservable<'T>
        Type : ChartType
        Width  : int
        Height : int
        Window : int option
    }

and [<JavaScript>] IRenderer<'T> =
    abstract member Body    : Element
    abstract member Render  : ChartState<'T> -> unit
    abstract member AddData : 'T -> unit
