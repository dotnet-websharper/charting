namespace WebSharper.Charting

open WebSharper

[<JavaScript>]
type Chart =

    static member Line dataset =
        GenericChart(Renderers.Default(), Stream.FromList dataset)
    
    static member Line dataset =
        GenericChart(Renderers.Default(), dataset)
    