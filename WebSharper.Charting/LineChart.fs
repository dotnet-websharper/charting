namespace WebSharper.Charting

open WebSharper

[<JavaScript>]
type LineChart (labels, dataset) =
    inherit Chart ()

    new (dataset) = LineChart (Seq.empty, dataset)

    member x.Labels  : string seq =
        if Seq.isEmpty labels then
            Seq.map string dataset
        else
            labels
    member x.Dataset : float seq  = dataset

    override x.Render () =
        ChartJs.LineChartData(
            Labels = Seq.toArray x.Labels,
            Datasets =
                [|
                    ChartJs.LineChartDataset(
                        FillColor        = Color.ToString (Color.Translucent x.Color 0.2),
                        StrokeColor      = Color.ToString x.Color,
                        PointColor       = Color.ToString x.Color,
                        PointStrokeColor = Color.ToString Color.White,
                        Data             = Seq.toArray x.Dataset
                    )
                |]
        )
        |> ChartJs.Chart(x.Context).Line
        |> ignore
