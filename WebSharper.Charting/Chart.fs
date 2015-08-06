namespace WebSharper.Charting

open WebSharper
open IntelliFactory.Reactive

[<JavaScript>]
type Chart =

    static member Line dataset =
        GenericChart(Renderers.LineChart(), BufferedStream.FromList dataset)
    
    static member Line (dataset : System.IObservable<float>) =
        let s = 
            Reactive.Select
            <| Reactive.Aggregate dataset (0, 0.0) (fun (s, _) c -> (s + 1, c))
            <| fun (a, b) -> (string a, b)
        GenericChart(Renderers.LineChart(), s)

    static member Line dataset =
        GenericChart(Renderers.LineChart(), dataset)

    static member Bar (dataset : System.IObservable<float>) =
        let s = 
            Reactive.Select
            <| Reactive.Aggregate dataset (0, 0.0) (fun (s, _) c -> (s + 1, c))
            <| fun (a, b) -> (string a, b)
        GenericChart(Renderers.BarChart(), s)

    static member Bar dataset =
        GenericChart(Renderers.BarChart(), dataset)

    static member Pie (dataset : System.IObservable<ChartJs.PolarAreaChartDataset>) =
        let s = 
            Reactive.Aggregate dataset (0, null) (fun (s, _) c -> (s + 1, c))
        GenericChart(Renderers.PieChart(), s)

    static member Pie dataset =
        GenericChart(Renderers.PieChart(), dataset)

    static member Area (dataset : System.IObservable<ChartJs.PolarAreaChartDataset>) =
        let s = 
            Reactive.Aggregate dataset (0, null) (fun (s, _) c -> (s + 1, c))
        GenericChart(Renderers.AreaChart(), s)

    static member Area dataset =
        GenericChart(Renderers.AreaChart(), dataset)

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
