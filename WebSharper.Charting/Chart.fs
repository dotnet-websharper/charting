namespace WebSharper.Charting

open WebSharper
open WebSharper.JavaScript

type Pagelet = Html.Client.Pagelet

[<AbstractClass; JavaScript>]
type Chart<'T when 'T :> Chart<'T>> (width, height, color) =
    inherit Pagelet ()

    let canvas =
        JS.Document.CreateElement "canvas"
        |> Canvas.Resize (width, height)
        |> (fun canvas ->
            canvas :> Dom.Node
        )
    override x.Body = canvas

    new () = Chart (400, 300, Color.RGBa (0, 0, 255))

    member internal x.Context = Canvas.GetContext canvas
    
    member x.Width  = width
    member x.Height = height
    member x.Color  = color

    abstract member WithDimension : int * int -> 'T
    abstract member WithColor     : (float -> string) -> 'T

[<JavaScript>]
module Chart =
    
    let WithDimension dimension (chart : 'T when 'T :> Chart<'T>) =
        chart.WithDimension dimension

    let WithColor color (chart : 'T when 'T :> Chart<'T>) =
        chart.WithColor color
