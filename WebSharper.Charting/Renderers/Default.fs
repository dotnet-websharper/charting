namespace WebSharper.Charting

open WebSharper
open WebSharper.Html.Client
open WebSharper.JavaScript

[<JavaScript>]
module Renderers =
    
    module private ChartTypes =
        let getChartJsConfig = function
            | Line ->
                ChartJs.LineChartConfiguration(
                    BezierCurve = false,
                    DatasetFill = false
                )
            | Pie ->
                ChartJs.PieChartConfiguration(
                    BezierCurve = false,
                    DatasetFill = false
                )

        let toChartJsChart context (options : ChartJs.GlobalChartConfiguration) = function
            | Line -> 
                let data = 
                    ChartJs.LineChartData(
                        Labels = Array.empty,
                        Datasets = [|ChartJs.LineChartDataset(Data = Array.empty)|])
                ChartJs.Chart(context).Line(data, options)

//            | Pie
//            | Doughnut
//            | Radar

    type ChartJsRenderer () =
        let canvas = Canvas []

        let mutable chart = null
        let mutable pc = 0
        let mutable window = None

        interface IRenderer<string * float> with
            member this.Body = canvas    

            member this.Render cs =
                canvas.Body?width  <- cs.Width
                canvas.Body?height <- cs.Height

                window <- cs.Window

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
                if window.IsSome then
                    if pc >= window.Value then chart.RemoveData()

                pc <- pc + 1

                chart.AddData([| data |], label)