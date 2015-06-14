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

    member internal x.Context = Canvas.GetContext x.Body
    
    member val Color = Color.RGBa (0, 0, 255, 1.0) with get, set

    static member WithDimension dimension (chart : #Chart) =
        Canvas.Resize dimension chart.Context.Canvas |> ignore

        chart
    
    static member WithColor color (chart : #Chart) =
        chart.Color <- color

        chart
