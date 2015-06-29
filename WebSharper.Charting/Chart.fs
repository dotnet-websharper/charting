namespace WebSharper.Charting

open WebSharper

[<JavaScript>]
type Chart =

    static member Line dataset =
        GenericChart(Renderers.Default(), BufferedStream.FromList dataset)
    
    static member Line dataset =
        GenericChart(Renderers.Default(), dataset)
    
    static member WithDimension (width, height) (chart : GenericChart<_>) =
        { chart.State with
            Width  = width
            Height = height }
        |> GenericChart.FromState
