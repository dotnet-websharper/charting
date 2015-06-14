namespace WebSharper.Charting

open WebSharper

[<JavaScript>]
module Color =

    type private Color = int * int * int * float

    let RGBa (color : Color) = color

    let Translucent ((r, g, b, _) : Color) a =
        RGBa (r, g, b, a)

    let ToString ((r, g, b, a) : Color) =
        "rgba(" + string r + ", " + string g + ", " + string b + ", " + string a + ")"

    // Constants
    let White =
        RGBa (255, 255, 255, 1.0)

    let Green =
        RGBa (0, 255, 0, 1.0)
