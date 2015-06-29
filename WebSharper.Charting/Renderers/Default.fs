namespace WebSharper.Charting

open WebSharper
open WebSharper.Html.Client
open WebSharper.JavaScript

[<JavaScript>]
module Renderers =
    
    type Default () =
        let canvas = Canvas []
        let mutable chart = null
        let mutable pc = 0

        interface IRenderer<string * float> with
            member this.Body = canvas    

            member this.Render cs =
                canvas.Body?width  <- cs.Width
                canvas.Body?height <- cs.Height
                
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

            member this.AddData((label, data)) =
                chart.AddData([| data |], label)

            member this.RemoveData () =
                chart.RemoveData()