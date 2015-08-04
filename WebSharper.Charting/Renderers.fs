namespace WebSharper.Charting

open System
open WebSharper

[<JavaScript>]
module Renderers =
    open WebSharper.JavaScript
    open WebSharper.Html.Client

    open Charts

    type RenderConfig =
        { Size : Size }
    
    let private defaultSize = Pervasives.Size(500, 200)

    type PolarChartType =
        | PolarArea of ChartJs.PolarAreaChartConfiguration
        | Pie of ChartJs.PieChartConfiguration
        | Doughnut of ChartJs.DoughnutChartConfiguration

    module internal ChartJsInternal =
        let private withNewCanvas (size : Size) k =
            let (Size (width, height)) = size
            Canvas [ Attr.Width <| string width; Attr.Height <| string height ]
            |>! OnAfterRender (fun c ->
                let ctx = (As<CanvasElement> c.Body).GetContext("2d")
                k c ctx)

        let private mkInitial dataSet window =
            match dataSet with
            | DataType.Live l -> [||]
            | DataType.Static s -> 
                window
                |> Option.fold (fun s w ->
                    let skp = s.Length - w
                    if skp >= s.Length then [||]
                    elif skp <= 0 then s
                    else s.[skp..]
                ) (Seq.toArray s)

        let private onEvent dataSet window remove add =
            match dataSet with
            | DataType.Live o ->
                let size = ref 0
                o.Add <| fun data ->
                    window |> Option.iter (fun window -> if !size >= window then remove ())
                    add data !size
                    incr size
            | _ -> ()

        let RenderLineChart (chart : LineChart) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let initial = mkInitial chart.DataSet window 

                let data =
                    ChartJs.LineChartData(
                        Labels   = (initial |> Array.map fst),
                        Datasets = 
                            [| ChartJs.LineChartDataset(
                                Label = chart.Config.Title,
                                FillColor = (string chart.Config.FillColor),
                                StrokeColor = (string chart.Config.StrokeColor),
                                PointColor = (string chart.Config.PointColor),
                                Data = (initial |> Array.map snd)) 
                            |])

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.LineChartConfiguration(
                        BezierCurve = false,
                        DatasetFill = false)

                let rendered = ChartJs.Chart(ctx).Line(data, options)

                onEvent chart.DataSet window
                <| fun () -> rendered.RemoveData ()
                <| fun (label, data) _ -> rendered.AddData([|data|], label)

        let RenderBarChart (chart : BarChart) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let initial = mkInitial chart.DataSet window

                let data =
                    ChartJs.BarChartData(
                        Labels   = (initial |> Array.map fst),
                        Datasets = 
                            [| ChartJs.BarChartDataset(
                                Label = chart.Config.Title,
                                FillColor = (string chart.Config.FillColor),
                                StrokeColor = (string chart.Config.StrokeColor),
                                Data = (initial |> Array.map snd)) 
                            |])

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.BarChartConfiguration(
                        BarShowStroke = true)

                let rendered = ChartJs.Chart(ctx).Bar(data, options)

                onEvent chart.DataSet window
                <| fun () -> rendered.RemoveData ()
                <| fun (label, data) _ -> rendered.AddData([|data|], label)

        let RenderRadarChart (chart : RadarChart) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let initial = mkInitial chart.DataSet window

                let data =
                    ChartJs.RadarChartData(
                        Labels   = (initial |> Array.map fst),
                        Datasets = 
                            [| ChartJs.RadarChartDataset(
                                Label = chart.Config.Title,
                                FillColor = (string chart.Config.FillColor),
                                StrokeColor = (string chart.Config.StrokeColor),
                                PointColor = (string chart.Config.PointColor),
                                Data = (initial |> Array.map snd)) 
                            |])

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.RadarChartConfiguration(
                        DatasetFill = true,
                        DatasetStroke = true)

                let rendered = ChartJs.Chart(ctx).Radar(data, options)

                onEvent chart.DataSet window
                <| fun () -> rendered.RemoveData ()
                <| fun (label, data) _ -> rendered.AddData([|data|], label)

        let RenderPolarAreaChart (chart : GenericPolarAreaChart<_>) size typ =
            withNewCanvas size <| fun canvas ctx ->
                let initial = mkInitial chart.DataSet None
                let convert e =
                    match typ with
                    | PolarChartType.PolarArea _ ->
                        ChartJs.PolarAreaChartDataset(
                            Color = string e.Color,
                            Highlight = string e.Highlight,
                            Value = e.Value,
                            Label = e.Label)
                    | PolarChartType.Pie _ ->
                        ChartJs.PieChartDataset(
                            Color = string e.Color,
                            Highlight = string e.Highlight,
                            Value = e.Value,
                            Label = e.Label) :> _
                    | PolarChartType.Doughnut _ ->
                        ChartJs.DoughnutChartDataset(
                            Color = string e.Color,
                            Highlight = string e.Highlight,
                            Value = e.Value,
                            Label = e.Label) :> _

                let data =
                    initial
                    |> Array.map convert

                let rendered =
                    match typ with
                    | PolarChartType.PolarArea opts ->
                        ChartJs.Chart(ctx).PolarArea(data, opts)
                    | PolarChartType.Pie opts ->
                        let d = As<ChartJs.PieChartDataset []> data 
                        ChartJs.Chart(ctx).Pie(d, opts) :> _
                    | PolarChartType.Doughnut opts ->
                        let d = As<ChartJs.DoughnutChartDataset []> data 
                        ChartJs.Chart(ctx).Doughnut(d, opts) :> _

                onEvent chart.DataSet None
                <| fun () -> ()
                <| fun pd idx -> rendered.AddData(convert pd, idx)

        let RenderCombinedLineChart (chart : CompisiteChart<LineChart>) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let labels =
                    chart.Charts
                    |> Seq.choose (fun chart ->
                        match chart.DataSet with
                        | Static s -> Some <| Seq.map fst s
                        | Live _ -> None)
                    |> Seq.maxBy Seq.length

                let data =
                    ChartJs.LineChartData(
                        Labels   = (labels |> Seq.toArray),
                        Datasets = 
                            (chart.Charts
                            |> Seq.map (fun chart ->
                                let initials = mkInitial chart.DataSet window
                                ChartJs.LineChartDataset(
                                    Label = chart.Config.Title,
                                    FillColor = (string chart.Config.FillColor),
                                    StrokeColor = (string chart.Config.StrokeColor),
                                    PointColor = (string chart.Config.PointColor),
                                    Data = (initials |> Array.map snd))
                            )
                            |> Seq.toArray)
                    )

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.LineChartConfiguration(
                        BezierCurve = false,
                        DatasetFill = false)

                ChartJs.Chart(ctx).Line(data, options) |> ignore

    type ChartJs =
        static member Render(chart : Charts.LineChart,
                             ?Size : Size,
                             ?Config : ChartJs.LineChartConfiguration,
                             ?Window : int) =
            ChartJsInternal.RenderLineChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.BarChart,
                             ?Size : Size,
                             ?Config : ChartJs.BarChartConfiguration,
                             ?Window : int) =
            ChartJsInternal.RenderBarChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.RadarChart,
                             ?Size : Size,
                             ?Config : ChartJs.RadarChartConfiguration,
                             ?Window : int) =
            ChartJsInternal.RenderRadarChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.PieChart,
                             ?Size : Size,
                             ?Config : ChartJs.PieChartConfiguration) =
            let typ = PolarChartType.Pie <| defaultArg Config (ChartJs.PieChartConfiguration())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ

        static member Render(chart : Charts.DoughnutChart,
                             ?Size : Size,
                             ?Config : ChartJs.DoughnutChartConfiguration) =
            let typ = PolarChartType.Doughnut <| defaultArg Config (ChartJs.DoughnutChartConfiguration())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ

        static member Render(chart : Charts.PolarAreaChart,
                             ?Size : Size,
                             ?Config : ChartJs.PolarAreaChartConfiguration) =
            let typ = PolarChartType.PolarArea <| defaultArg Config (ChartJs.PolarAreaChartConfiguration())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ

        static member Render(chart : Charts.CompisiteChart<LineChart>,
                             ?Size : Size,
                             ?Config : ChartJs.LineChartConfiguration,
                             ?Window : int) =
            ChartJsInternal.RenderCombinedLineChart chart (defaultArg Size defaultSize) Config Window

        
