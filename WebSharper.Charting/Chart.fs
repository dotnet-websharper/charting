namespace WebSharper.Charting

open WebSharper
open WebSharper.JavaScript

type   Pagelet = Html.Client.Pagelet 
module Tags    = Html.Client.Tags

[<JavaScript>]
type Chart () =
    inherit Pagelet ()

    let canvas =
        JS.Document.CreateElement "canvas"
        |> Canvas.Resize (400, 300)
        |> (fun canvas ->
            canvas :> Dom.Node
        )

    override x.Body =
        Tags.Div [
            match x.Title with
            | Some title ->
                yield Tags.H2 [ Tags.Text title ]
            | _ ->
                ()
        ]
        |> (fun con ->
            con.Body.AppendChild canvas |> ignore
            con.Body
        )

    member internal x.Context = Canvas.GetContext canvas
    
    member val Color = Color.RGBa (0, 0, 255, 1.0) with get, set
    member val Title : string option = None with get, set

    static member WithDimension dimension (chart : #Chart) =
        Canvas.Resize dimension chart.Context.Canvas |> ignore

        chart
    
    static member WithColor color (chart : #Chart) =
        chart.Color <- color

        chart

    static member WithTitle title (chart : #Chart) =
        chart.Title <- Some title

        chart
