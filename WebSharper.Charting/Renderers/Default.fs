namespace WebSharper.Charting

open WebSharper
open WebSharper.Html.Client
open WebSharper.JavaScript

[<JavaScript>]
module Renderers =
    
    type Default () =
        let canvas =
            Canvas [
                Width  "400"
                Height "300"
            ]
        let mutable chart = null

        interface IRenderer<string * float> with
            member this.Body = canvas    

            member this.Render cs =
                match cs.Type with
                | Line ->
                    let data =
                        ChartJs.LineChartData(
                            Labels   = Array.empty,
                            Datasets =
                                [|
                                    ChartJs.LineChartDataset(Data = Array.empty)
                                |]
                        )

                    let options =
                        ChartJs.LineChartConfiguration(
                            BezierCurve = false,
                            DatasetFill = false
                        )

                    let context =
                        (As<CanvasElement> canvas.Body).GetContext("2d")

                    chart <- ChartJs.Chart(context).Line(data, options)

            member this.AddData =
                function
                | (label, data) ->
                    chart.AddData([| data |], label)
