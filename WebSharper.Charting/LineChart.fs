namespace WebSharper.Charting

open WebSharper

[<JavaScript>]
type LineChart private (width, height, color, labels, dataset) =
    inherit Chart<LineChart> (width, height, color)

    new (labels, dataset) = LineChart (400, 300, Color.RGBa (0, 0, 255), labels, dataset)

    member x.Labels  : string seq = labels
    member x.Dataset : float seq  = dataset

    override x.Render () =
        ChartJs.LineChartData(
            Labels   = Seq.toArray x.Labels,
            Datasets =
                [|
                    ChartJs.LineChartDataset(
                        FillColor        = x.Color 0.2,
                        StrokeColor      = x.Color 1.0,
                        PointColor       = x.Color 1.0,
                        PointStrokeColor = Color.White,
                        Data             = Seq.toArray x.Dataset
                    )
                |]
        )
        |> ChartJs.Chart(x.Context).Line
        |> ignore

    override x.WithDimension (width, height) =
        LineChart (width, height, x.Color, x.Labels, x.Dataset)

    override x.WithColor color =
        LineChart (x.Width, x.Height, color, x.Labels, x.Dataset)
