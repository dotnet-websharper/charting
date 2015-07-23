namespace WebSharper.Charting

open System
open WebSharper

[<JavaScript>]
module Renderers =
    open WebSharper.JavaScript
    open WebSharper.Html.Client

    open Charts

    [<Interface>]
    type IRenderer<'T when 'T :> GenericChart<'T>> =
        abstract member Render : 'T -> Element

    module ChartJs =
        open WebSharper.ChartJs

        let private withNewCanvas (size : Size) k =
            let (Size (width, height)) = size
            Canvas [ Attr.Width <| string width; Attr.Height <| string height ]
            |>! OnAfterRender (fun c ->
                let ctx = (As<CanvasElement> c.Body).GetContext("2d")
                k c ctx)
            
        type LineChartRenderer(canvasSize : Size, ?options : ChartJs.LineChartConfiguration) =
            interface IRenderer<LineChart> with

                override x.Render chart =
                    withNewCanvas canvasSize <| fun canvas ctx ->
                        let data =
                            ChartJs.LineChartData(
                                Labels   = (chart.DataSet |> Seq.map fst |> Seq.toArray),
                                Datasets = 
                                    [| ChartJs.LineChartDataset(
                                        Label = chart.Config.Title,
                                        FillColor = (string chart.Config.FillColor),
                                        StrokeColor = (string chart.Config.StrokeColor),
                                        PointColor = (string chart.Config.PointColor),
                                        Data = (chart.DataSet |> Seq.map snd |> Seq.toArray)) 
                                    |])

                        let options =
                            defaultArg
                            <| options
                            <| ChartJs.LineChartConfiguration(
                                BezierCurve = false,
                                DatasetFill = false)

                        ChartJs.Chart(ctx).Line(data, options) |> ignore

            [<Name "__Render">]
            member x.Render chart = (x :> IRenderer<LineChart>).Render chart

        module Live =
            type LineChartRenderer(canvasSize : Size, ?window : int, ?options : ChartJs.LineChartConfiguration) =
                let window = defaultArg window 50

                interface IRenderer<LiveLineChart> with

                    override x.Render chart =
                        withNewCanvas canvasSize <| fun canvas ctx ->
                            let data =
                                ChartJs.LineChartData(
                                    Labels   = [||],
                                    Datasets = 
                                        [| ChartJs.LineChartDataset(
                                            Label = chart.Config.Title,
                                            FillColor = (string chart.Config.FillColor),
                                            StrokeColor = (string chart.Config.StrokeColor),
                                            PointColor = (string chart.Config.PointColor),
                                            Data = [||])
                                        |])

                            let options =
                                defaultArg
                                <| options
                                <| ChartJs.LineChartConfiguration(
                                    BezierCurve = false,
                                    DatasetFill = false)

                            let ch = ChartJs.Chart(ctx).Line(data, options)
                            let size = ref 0

                            chart.DataSet.Add <| fun (label, data) ->
                                if !size >= window then ch.RemoveData()
                                ch.AddData([|data|], label)
                                incr size

                [<Name "__Render">]
                member x.Render chart = (x :> IRenderer<LiveLineChart>).Render chart
