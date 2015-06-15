namespace WebSharper.Charting

open WebSharper

[<JavaScript>]
type RadarChart private (width, height, color, labels, dataset) =
    inherit Chart<RadarChart> (width, height, color)

    new (labels, dataset) = RadarChart (400, 300, Color.RGBa (0, 0, 255), labels, dataset)

    member x.Labels  : string seq = labels
    member x.Dataset : float seq  = dataset

    override x.Render () =
        ChartJs.RadarChartData(
            Labels   = Seq.toArray x.Labels,
            Datasets =
                [|
                    ChartJs.RadarChartDataset(
                        FillColor        = x.Color 0.2,
                        StrokeColor      = x.Color 1.0,
                        PointColor       = x.Color 1.0,
                        PointStrokeColor = Color.White,
                        Data             = Seq.toArray x.Dataset
                    )
                |]
        )
        |> ChartJs.Chart(x.Context).Radar
        |> ignore

    override x.WithDimension (width, height) =
        RadarChart (width, height, x.Color, x.Labels, x.Dataset)

    override x.WithColor color =
        RadarChart (x.Width, x.Height, color, x.Labels, x.Dataset)
