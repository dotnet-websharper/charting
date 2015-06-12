namespace WebSharper.Charting

open WebSharper
open WebSharper.JavaScript

type private Pagelet = Html.Client.Pagelet

[<JavaScript>]
type Chart () =
    inherit Pagelet ()

    let canvas =
        JS.Document.CreateElement "canvas"
        |> Canvas.Resize (400, 300)
        |> (fun canvas ->
            canvas :> Dom.Node
        )
    override x.Body = canvas

    member internal x.Context : CanvasRenderingContext2D = Canvas.GetContext x.Body

    static member WithDimension di (chart : #Chart) =
        Canvas.Resize di chart.Context.Canvas |> ignore
        
        chart
