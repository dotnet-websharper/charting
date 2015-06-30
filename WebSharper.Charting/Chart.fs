namespace WebSharper.Charting

open WebSharper
open IntelliFactory.Reactive

[<JavaScript>]
type Chart =

    static member Line dataset =
        GenericChart(Renderers.Default(), BufferedStream.FromList dataset)
    
    static member Line (dataset : System.IObservable<float>) =
        let s = 
            Reactive.Select
            <| Reactive.Aggregate dataset (0, 0.0) (fun (s, _) c -> (s + 1, c))
            <| fun (a, b) -> (string a, b)
        GenericChart(Renderers.Default(), s)

    static member Line dataset =
        GenericChart(Renderers.Default(), dataset)

    static member WithWindow window (chart : GenericChart<_>) =
        { chart.State with
            Window = Some window }
        |> GenericChart.FromState

    static member WithRenderer renderer (chart : GenericChart<_>) =
        { chart.State with
            Renderer = renderer }
        |> GenericChart.FromState
    
    static member WithDimension (width, height) (chart : GenericChart<_>) =
        { chart.State with
            Width  = width
            Height = height }
        |> GenericChart.FromState
