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
                    ChartJs.LineChartDataset(Data = Seq.toArray x.Dataset)
                |]
        )
        |> ChartJs.Chart(x.Context).Line
        |> ignore
