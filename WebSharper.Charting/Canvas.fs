namespace WebSharper.Charting

open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module internal Canvas =
    
    [<Inline "$canvas.getContext(\"2d\")">]
    let GetContext (canvas : Dom.Node) = X<CanvasRenderingContext2D>

    let Resize (width, height) (canvas : Dom.Element) =
        canvas?width  <- width
        canvas?height <- height

        canvas
