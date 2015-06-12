namespace WebSharper.Charting

open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module internal Canvas =
    
    [<Inline "$canvas.getContext(\"2d\")">]
    let GetContext (canvas : Dom.Node) = X<CanvasRenderingContext2D>

    let Resize (width : int, height : int) (canvas : Dom.Element) =
        canvas.SetAttribute("width", string width)
        canvas.SetAttribute("height", string height)

        canvas
