namespace WebSharper.Charting

open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module Color =
    
    type private Color = int * int * int

    let RGBa ((r, g, b) : Color) a =
        "rgba(" + string r + ", " + string g + ", " + string b + ", " + string a + ")"

    let White =
        RGBa (255, 255, 255) 1.0
