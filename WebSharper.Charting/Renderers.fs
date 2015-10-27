namespace WebSharper.Charting

open System
open WebSharper

[<JavaScript>]
module Renderers =
    open WebSharper.JavaScript
    open WebSharper.UI.Next.Html
    open WebSharper.UI.Next.Client

    open IntelliFactory.Reactive

    open Charts

    let private defaultSize = Pervasives.Size(500, 200)

    type internal PolarChartType =
        | PolarArea of ChartJs.PolarAreaChartConfiguration
        | Pie of ChartJs.PieChartConfiguration
        | Doughnut of ChartJs.DoughnutChartConfiguration

    module internal ChartJsInternal =
        type private BatchUpdater(?interval : int, ?maxCount : int) =
            let interval = defaultArg interval 75
            let maxCount = defaultArg maxCount 10

            let handle : JS.Handle option ref = ref None
            let count = ref 0

            member x.Update updater =
                let doUpdate () =
                    handle := None
                    count := 0
                    updater ()

                !handle |> Option.iter JS.ClearTimeout
                if !count < maxCount then
                    incr count

                    handle :=
                        JS.SetTimeout
                        <| doUpdate
                        <| interval
                        |> Some
                else doUpdate ()

        let private registerUpdater (mChart : IMutableChart<float, int>) upd fin =
            let bu = new BatchUpdater()
            mChart.OnUpdate <| fun (i, d) ->
                upd(i,d)
                bu.Update fin

        let private withNewCanvas (size : Size) k =
            let (Size (width, height)) = size
            canvasAttr [ 
                attr.width <| string width
                attr.height <| string height
                on.afterRender <| fun el ->
                    let ctx = (As<CanvasElement> el).GetContext("2d")
                    k el ctx
            ] []

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
                    window |> Option.iter (fun window -> if !size >= window then remove window !size)
                    add !size data
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
                                FillColor = (string chart.SeriesConfig.FillColor),
                                StrokeColor = (string chart.SeriesConfig.StrokeColor),
                                PointColor = (string chart.ColorConfig.PointColor),
                                PointHighlightFill = (string chart.ColorConfig.PointHighlightFill),
                                PointHighlightStroke = (string chart.ColorConfig.PointHighlightStroke),
                                PointStrokeColor = (string chart.ColorConfig.PointStrokeColor),
                                Data = (initial |> Array.map snd)) 
                            |])

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.LineChartConfiguration(
                        BezierCurve = false,
                        DatasetFill = false)

                let rendered = ChartJs.Chart(ctx).Line(data, options)

                registerUpdater (chart :> IMutableChart<float, int>)
                <| fun (i, d) ->
                    let ds : obj [] = rendered?datasets
                    let s : obj [] = ds.[0]?points
                    s.[i]?value <- d s.[i]?value
                <| rendered.Update

                onEvent chart.DataSet window
                <| fun _ _ -> rendered.RemoveData ()
                <| fun _ (label, data) -> rendered.AddData([|data|], label)

        let RenderBarChart (chart : BarChart) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let initial = mkInitial chart.DataSet window

                let data =
                    ChartJs.BarChartData(
                        Labels   = (initial |> Array.map fst),
                        Datasets = 
                            [| ChartJs.BarChartDataset(
                                Label = chart.Config.Title,
                                FillColor = (string chart.SeriesConfig.FillColor),
                                StrokeColor = (string chart.SeriesConfig.StrokeColor),
                                Data = (initial |> Array.map snd)) 
                            |])

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.BarChartConfiguration(
                        BarShowStroke = true)

                let rendered = ChartJs.Chart(ctx).Bar(data, options)

                registerUpdater (chart :> IMutableChart<float, int>)
                <| fun (i, d) ->
                    let ds : obj [] = rendered?datasets
                    let s : obj [] = ds.[0]?bars
                    s.[i]?value <- d s.[i]?value
                <| rendered.Update

                onEvent chart.DataSet window
                <| fun _ _ -> rendered.RemoveData ()
                <| fun _ (label, data) -> rendered.AddData([|data|], label)

        let RenderRadarChart (chart : RadarChart) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let initial = mkInitial chart.DataSet window

                let data =
                    ChartJs.RadarChartData(
                        Labels   = (initial |> Array.map fst),
                        Datasets = 
                            [| ChartJs.RadarChartDataset(
                                Label = chart.Config.Title,
                                FillColor = (string chart.SeriesConfig.FillColor),
                                StrokeColor = (string chart.SeriesConfig.StrokeColor),
                                PointColor = (string chart.ColorConfig.PointColor),
                                PointHighlightFill = (string chart.ColorConfig.PointHighlightFill),
                                PointHighlightStroke = (string chart.ColorConfig.PointHighlightStroke),
                                PointStrokeColor = (string chart.ColorConfig.PointStrokeColor),
                                Data = (initial |> Array.map snd)) 
                            |])

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.RadarChartConfiguration(
                        DatasetFill = true,
                        DatasetStroke = true)

                let rendered = ChartJs.Chart(ctx).Radar(data, options)

                registerUpdater (chart :> IMutableChart<float, int>)
                <| fun (i, d) ->
                    let ds : obj [] = rendered?datasets
                    let s : obj [] = ds.[0]?points
                    s.[i]?value <- d s.[i]?value
                <| rendered.Update

                onEvent chart.DataSet window
                <| fun _ _ -> rendered.RemoveData ()
                <| fun _ (label, data) -> rendered.AddData([|data|], label)

        let RenderPolarAreaChart (chart : IPolarAreaChart<_>) size typ window =
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

                (chart :> IMutableChart<float, int>).OnUpdate <| fun (i, d) ->
                    let s : obj [] = rendered?segments
                    s.[i]?value <- d s.[i]?value
                    rendered.Update()

                onEvent chart.DataSet window
                <| fun _ _ -> rendered.RemoveData 0
                <| fun size data -> rendered.AddData(convert data, size)

        let private extractStreams dataSet =
            dataSet
            |> Seq.mapi (fun i data -> 
                match data with
                | DataType.Live s -> 
                    Some <| Reactive.Select s (fun d -> (i, d))
                | DataType.Static _ -> None)
            |> Seq.choose id
            |> Reactive.SequenceOnlyNew

        let private onCombinedEvent (streams : IObservable<seq<int * (string * float)>>) l window remove add =
            let size = ref 0
            streams.Add <| fun data ->
                window |> Option.iter (fun window -> if !size >= window then remove window !size)
                let arr = [| for i in 1 .. l -> 0. |]
                data
                |> Seq.iter (fun (i, (d, l)) -> arr.[i] <- l)
                    
                data |> Seq.headOption |> Option.iter (fun (_, (label, _)) -> add !size (arr, label))
                    
                incr size

        let RenderCombinedLineChart (chart : CompositeChart<LineChart>) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let labels =
                    chart.Charts
                    |> Seq.choose (fun chart ->
                        match chart.DataSet with
                        | Static s as dt -> Some <| mkInitial dt window
                        | Live _ -> None)
                    |> Seq.map (Seq.map fst)
                    |> fun e ->
                        if Seq.length e > 0 then
                            Seq.maxBy Seq.length e
                        else Seq.empty

                let data =
                    ChartJs.LineChartData(
                        Labels   = (labels |> Seq.toArray),
                        Datasets = 
                            (chart.Charts
                            |> Seq.map (fun chart ->
                                let initials = mkInitial chart.DataSet window
                                ChartJs.LineChartDataset(
                                    Label = chart.Config.Title,
                                    FillColor = (string chart.SeriesConfig.FillColor),
                                    StrokeColor = (string chart.SeriesConfig.StrokeColor),
                                    PointColor = (string chart.ColorConfig.PointColor),
                                    PointHighlightFill = (string chart.ColorConfig.PointHighlightFill),
                                    PointHighlightStroke = (string chart.ColorConfig.PointHighlightStroke),
                                    PointStrokeColor = (string chart.ColorConfig.PointStrokeColor),
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

                let rendered = 
                    ChartJs.Chart(ctx).Line(data, options)

                chart.Charts
                |> Seq.iteri (fun i chart ->
                    registerUpdater (chart :> IMutableChart<float, int>)
                    <| fun (j, d) ->
                        let ds : obj [] = rendered?datasets
                        let s : obj [] = ds.[i]?points
                        s.[j]?value <- d s.[j]?value
                    <| rendered.Update
                )

                let streams =
                    chart.Charts
                    |> Seq.map (fun chart -> chart.DataSet)
                    |> extractStreams

                onCombinedEvent streams (Seq.length chart.Charts) window
                <| fun _ _ -> rendered.RemoveData()
                <| fun _ (arr, label) ->
                    rendered.AddData(arr, label)

        let RenderCombinedBarChart (chart : CompositeChart<BarChart>) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let labels =
                    chart.Charts
                    |> Seq.choose (fun chart ->
                        match chart.DataSet with
                        | Static s -> Some <| Seq.map fst s
                        | Live _ -> None)
                    |> fun e ->
                        if Seq.length e > 0 then
                            Seq.maxBy Seq.length e
                        else Seq.empty

                let data =
                    ChartJs.BarChartData(
                        Labels   = (labels |> Seq.toArray),
                        Datasets = 
                            (chart.Charts
                            |> Seq.map (fun chart ->
                                let initials = mkInitial chart.DataSet window
                                ChartJs.BarChartDataset(
                                    Label = chart.Config.Title,
                                    FillColor = (string chart.SeriesConfig.FillColor),
                                    StrokeColor = (string chart.SeriesConfig.StrokeColor),
                                    Data = (initials |> Array.map snd)) 
                            )
                            |> Seq.toArray)
                    )

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.BarChartConfiguration(BarShowStroke = true)

                let rendered = 
                    ChartJs.Chart(ctx).Bar(data, options)

                chart.Charts
                |> Seq.iteri (fun i chart ->
                    registerUpdater (chart :> IMutableChart<float, int>)
                    <| fun (j, d) ->
                        let ds : obj [] = rendered?datasets
                        let s : obj [] = ds.[i]?bars
                        s.[j]?value <- d s.[j]?value
                    <| rendered.Update
                )

                let streams =
                    chart.Charts
                    |> Seq.map (fun chart -> chart.DataSet)
                    |> extractStreams

                onCombinedEvent streams (Seq.length chart.Charts) window
                <| fun _ _ -> rendered.RemoveData()
                <| fun _ (arr, label) ->
                    rendered.AddData(arr, label)

        let RenderCombinedRadarChart (chart : CompositeChart<RadarChart>) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let labels =
                    chart.Charts
                    |> Seq.choose (fun chart ->
                        match chart.DataSet with
                        | Static s -> Some <| Seq.map fst s
                        | Live _ -> None)
                    |> fun e ->
                        if Seq.length e > 0 then
                            Seq.maxBy Seq.length e
                        else Seq.empty

                let data =
                    ChartJs.RadarChartData(
                        Labels   = (labels |> Seq.toArray),
                        Datasets = 
                            (chart.Charts
                            |> Seq.map (fun chart ->
                                let initials = mkInitial chart.DataSet window
                                ChartJs.RadarChartDataset(
                                    Label = chart.Config.Title,
                                    FillColor = (string chart.SeriesConfig.FillColor),
                                    StrokeColor = (string chart.SeriesConfig.StrokeColor),
                                    PointColor = (string chart.ColorConfig.PointColor),
                                    PointHighlightFill = (string chart.ColorConfig.PointHighlightFill),
                                    PointHighlightStroke = (string chart.ColorConfig.PointHighlightStroke),
                                    PointStrokeColor = (string chart.ColorConfig.PointStrokeColor),
                                    Data = (initials |> Array.map snd))
                            )
                            |> Seq.toArray)
                    )

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.RadarChartConfiguration(
                        DatasetFill = true,
                        DatasetStroke = true)

                let rendered = 
                    ChartJs.Chart(ctx).Radar(data, options)

                chart.Charts
                |> Seq.iteri (fun i chart ->
                    registerUpdater (chart :> IMutableChart<float, int>)
                    <| fun (j, d) ->
                        let ds : obj [] = rendered?datasets
                        let s : obj [] = ds.[i]?points
                        s.[j]?value <- d s.[j]?value
                    <| rendered.Update
                )

                let streams =
                    chart.Charts
                    |> Seq.map (fun chart -> chart.DataSet)
                    |> extractStreams

                onCombinedEvent streams (Seq.length chart.Charts) window
                <| fun _ _ -> rendered.RemoveData()
                <| fun _ (arr, label) ->
                    rendered.AddData(arr, label)

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
                             ?Config : ChartJs.PieChartConfiguration,
                             ?Window : int) =
            let typ = PolarChartType.Pie <| defaultArg Config (ChartJs.PieChartConfiguration())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ Window

        static member Render(chart : Charts.DoughnutChart,
                             ?Size : Size,
                             ?Config : ChartJs.DoughnutChartConfiguration,
                             ?Window : int) =
            let typ = PolarChartType.Doughnut <| defaultArg Config (ChartJs.DoughnutChartConfiguration())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ Window

        static member Render(chart : Charts.PolarAreaChart,
                             ?Size : Size,
                             ?Config : ChartJs.PolarAreaChartConfiguration,
                             ?Window : int) =
            let typ = PolarChartType.PolarArea <| defaultArg Config (ChartJs.PolarAreaChartConfiguration())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ Window

        static member Render(chart : Charts.CompositeChart<LineChart>,
                             ?Size : Size,
                             ?Config : ChartJs.LineChartConfiguration,
                             ?Window : int) =
            ChartJsInternal.RenderCombinedLineChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.CompositeChart<BarChart>,
                             ?Size : Size,
                             ?Config : ChartJs.BarChartConfiguration,
                             ?Window : int) =
            ChartJsInternal.RenderCombinedBarChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.CompositeChart<RadarChart>,
                             ?Size : Size,
                             ?Config : ChartJs.RadarChartConfiguration,
                             ?Window : int) =
            ChartJsInternal.RenderCombinedRadarChart chart (defaultArg Size defaultSize) Config Window

