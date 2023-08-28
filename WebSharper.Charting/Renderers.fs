// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2018 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}
namespace WebSharper.Charting

open System
open WebSharper

[<JavaScript>]
module Renderers =
    open WebSharper.JavaScript
    open WebSharper.UI.Html
    open WebSharper.UI.Client

    open Charts

    let private defaultSize = Pervasives.Size(500, 200)
    
    [<Inline "$ds[$l] = $o">]
    let addNew (ds: obj []) (l: int) (o:obj) = X<unit>

    [<Inline "$ds.shift()">]
    let popFrom (ds: obj []) = X<unit>

    [<Inline "$ds.push($v)">]
    let pushTo (ds: obj []) v = X<unit>

    type internal PolarChartType =
        | PolarArea of ChartJs.PolarAreaChartOptions
        | Pie of ChartJs.PieDoughnutChartOptions
        | Doughnut of ChartJs.PieDoughnutChartOptions

    let internal withNewCanvas (size : Size) k =
        let (Size (width, height)) = size
        div [
            attr.width <| string width
            attr.height <| string height
            Attr.Style "width" (string width + "px")
            Attr.Style "height" (string height + "px")
        ] [
            canvas [ 
                on.afterRender <| fun el ->
                    let ctx = (As<CanvasElement> el).GetContext("2d")
                    (el :?> CanvasElement).Width <- width
                    (el :?> CanvasElement).Height <- height
                    k el ctx
            ] []
        ]

    let internal withNewDiv (size : Size) k =
        let (Size (width, height)) = size
        div [
            attr.width <| string width
            attr.height <| string height
            Attr.Style "width" (string width + "px")
            Attr.Style "height" (string height + "px")
            on.afterRender <| fun el ->
                k el
        ] []

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
                    ChartJs.ChartData(
                            Datasets =
                                [| ChartJs.LineChartDataSet(
                                    Label = chart.Config.Title,
                                    Fill = Union1Of2 chart.SeriesConfig.IsFilled,
                                    BackgroundColor = Union1Of2 (string chart.SeriesConfig.FillColor),
                                    BorderColor = Union1Of2 (string chart.SeriesConfig.StrokeColor),
                                    PointBackgroundColor = Union1Of2(string chart.ColorConfig.PointColor),
                                    PointHoverBackgroundColor = Union1Of2(string chart.ColorConfig.PointHighlightFill),
                                    PointHoverBorderColor = Union1Of2(string chart.ColorConfig.PointHighlightStroke),
                                    PointBorderColor = Union1Of2(string chart.ColorConfig.PointStrokeColor),
                                    Data = (initial |> Array.map snd)) 
                                |]
                    )

                data.Labels <- (initial |> Array.map fst)

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.Options()

                // options.

                let chartcreate =
                    ChartJs.ChartCreate(data, options)

                let rendered = ChartJs.Chart(canvas, chartcreate)

                registerUpdater (chart :> IMutableChart<float, int>)
                <| fun (i, d) ->
                    let data : obj = rendered?data
                    let ds : obj [] = data?datasets
                    let s : obj [] = ds.[0]?data
                    addNew s i (d (s.[i] :?> float))
                <| rendered.Update

                onEvent chart.DataSet window
                <| fun _ _ ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iter (fun d ->
                        popFrom (d?data : obj [])    
                    )
                    popFrom labels
                    rendered.Update()
                <| fun a (label, arr) ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iteri (fun i (d: obj) ->
                        let dd : obj[] = d?data
                        addNew (dd) (dd.Length) (arr)
                    )
                    addNew labels (labels.Length) (label)
                    rendered.Update()

        let RenderBarChart (chart : BarChart) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let initial = mkInitial chart.DataSet window

                let data =
                    ChartJs.ChartData(
                            Datasets =
                                [| ChartJs.BarChartDataSet(
                                    Label = chart.Config.Title,
                                    BackgroundColor = Union1Of2 (string chart.SeriesConfig.FillColor),
                                    BorderColor = Union1Of2 (string chart.SeriesConfig.StrokeColor),
                                    Data = (initial |> Array.map snd))
                                |]
                    )
                data.Labels <- (initial |> Array.map fst)

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.Options()

                let chartcreate =
                    ChartJs.ChartCreate(data, options)

                let rendered = ChartJs.Chart(canvas, chartcreate)

                registerUpdater (chart :> IMutableChart<float, int>)
                <| fun (i, d) ->
                    let data : obj = rendered?data
                    let ds : obj [] = data?datasets
                    let s : obj [] = ds.[0]?data
                    addNew s i (d (s.[i] :?> float))
                <| rendered.Update

                onEvent chart.DataSet window
                <| fun _ _ ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iter (fun d ->
                        popFrom (d?data : obj [])    
                    )
                    popFrom labels
                    rendered.Update()
                <| fun a (label, arr) ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iteri (fun i (d: obj) ->
                        let dd : obj[] = d?data
                        addNew (dd) (dd.Length) (arr)
                    )
                    addNew labels (labels.Length) (label)
                    rendered.Update()

        let RenderRadarChart (chart : RadarChart) size cfg window =
            withNewCanvas size <| fun canvas ctx ->
                let initial = mkInitial chart.DataSet window

                let data =
                    ChartJs.ChartData(
                            Datasets =
                                [| ChartJs.RadarChartDataSet(
                                    Label = chart.Config.Title,
                                    Fill = Union1Of2 chart.SeriesConfig.IsFilled,
                                    BackgroundColor = Union1Of2 (string chart.SeriesConfig.FillColor),
                                    BorderColor = Union1Of2 (string chart.SeriesConfig.StrokeColor),
                                    PointBackgroundColor = Union1Of2(string chart.ColorConfig.PointColor),
                                    PointHoverBackgroundColor = Union1Of2(string chart.ColorConfig.PointHighlightFill),
                                    PointHoverBorderColor = Union1Of2(string chart.ColorConfig.PointHighlightStroke),
                                    PointBorderColor = Union1Of2(string chart.ColorConfig.PointStrokeColor),
                                    Data = (initial |> Array.map snd)) 
                                |]
                    )

                data.Labels <- (initial |> Array.map fst)

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.RadarChartOptions()
                
                let chartcreate =
                    ChartJs.ChartCreate(data, options)

                let rendered = ChartJs.Chart(canvas, chartcreate)

                registerUpdater (chart :> IMutableChart<float, int>)
                <| fun (i, d) ->
                    let data : obj = rendered?data
                    let ds : obj [] = data?datasets
                    let s : obj [] = ds.[0]?data
                    addNew s i (d (s.[i] :?> float))
                <| rendered.Update

                onEvent chart.DataSet window
                <| fun _ _ ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iter (fun d ->
                        popFrom (d?data : obj [])    
                    )
                    popFrom labels
                    rendered.Update()
                <| fun a (label, arr) ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iteri (fun i (d: obj) ->
                        let dd : obj[] = d?data
                        addNew (dd) (dd.Length) arr
                    )
                    addNew labels (labels.Length) (label)
                    rendered.Update()

        let RenderPolarAreaChart (chart : IPolarAreaChart<_>) size typ window =
            withNewCanvas size <| fun canvas ctx ->
                let initial = mkInitial chart.DataSet None
                let toBGColor = 
                    initial
                    |> Array.map (fun e -> string e.Color)
                let toHBGColor = 
                    initial
                    |> Array.map (fun e -> string e.Highlight)
                let toValue = 
                    initial
                    |> Array.map (fun e -> float e.Value)
                let toLabel = 
                    initial
                    |> Array.map (fun e -> e.Label)
                let cc =
                    match typ with
                    | PolarChartType.PolarArea opts ->
                        let x =
                            ChartJs.ChartData (
                                Datasets =
                                    [|
                                        ChartJs.PolarAreaChartDataSet(
                                            Data = toValue,
                                            BackgroundColor = Union2Of2 toBGColor,
                                            HoverBackgroundColor = Union2Of2 toHBGColor
                                        )
                                    |]
                            )
                        x.Labels <- toLabel
                        ChartJs.ChartCreate(x, opts)
                    | PolarChartType.Pie opt ->
                        let x =
                            ChartJs.ChartData(
                                Datasets =
                                    [|
                                        ChartJs.PieChartDataSet(
                                            Data = toValue,
                                            BackgroundColor = Union2Of2 toBGColor,
                                            HoverBackgroundColor = Union2Of2 toHBGColor)
                                    |]
                            )
                        x.Labels <- toLabel
                        ChartJs.ChartCreate(x, opt)
                    | PolarChartType.Doughnut opt ->
                        let x =
                            ChartJs.ChartData(
                                Datasets =
                                    [|
                                        ChartJs.DoughnutChartDataSet(
                                            Data = toValue,
                                            BackgroundColor = Union2Of2 toBGColor,
                                            HoverBackgroundColor = Union2Of2 toHBGColor)
                                    |]
                            )
                        x.Labels <- toLabel
                        ChartJs.ChartCreate(x, opt)

                let rendered = ChartJs.Chart(canvas, cc)
                        
                onEvent chart.DataSet window
                <| fun _ _ ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iter (fun d ->
                        popFrom (d?data : obj [])    
                    )
                    popFrom labels
                    rendered.Update()
                <| fun a (polardata) ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iteri (fun i (d: obj) ->
                        let dd : obj[] = d?data
                        addNew (dd) (dd.Length) (polardata.Value)
                    )
                    addNew labels (labels.Length) polardata.Label
                    rendered.Update()

                (chart :> IMutableChart<float, int>).OnUpdate <| fun (i, d) ->
                    let data : obj = rendered?data
                    let ds : obj [] = data?datasets
                    let s : obj [] = ds.[0]?data
                    addNew s i (d (s.[i] :?> float))
                    rendered.Update()

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
                    ChartJs.ChartData(
                        Datasets =
                            (chart.Charts
                            |> Seq.map (fun chart ->
                                let initials = mkInitial chart.DataSet window
                                ChartJs.LineChartDataSet(
                                    Label = chart.Config.Title,
                                    Fill = Union1Of2 chart.SeriesConfig.IsFilled,
                                    BackgroundColor = Union1Of2 (string chart.SeriesConfig.FillColor),
                                    BorderColor = Union1Of2 (string chart.SeriesConfig.StrokeColor),
                                    PointBackgroundColor = Union1Of2(string chart.ColorConfig.PointColor),
                                    PointHoverBackgroundColor = Union1Of2(string chart.ColorConfig.PointHighlightFill),
                                    PointHoverBorderColor = Union1Of2(string chart.ColorConfig.PointHighlightStroke),
                                    PointBorderColor = Union1Of2(string chart.ColorConfig.PointStrokeColor),
                                    Data = (initials |> Array.map snd)) :> ChartJs.ADataSet
                            )
                            |> Seq.toArray)
                    )

                data.Labels <- (labels |> Seq.toArray)

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.Options()

                let chartcreate =
                    ChartJs.ChartCreate(data, options)
                
                let rendered = 
                    ChartJs.Chart(canvas, chartcreate)

                chart.Charts
                |> Seq.iteri (fun i chart ->
                    registerUpdater (chart :> IMutableChart<float, int>)
                    <| fun (j, d) ->
                        let data : obj = rendered?data
                        let ds : obj [] = data?datasets
                        let s : obj [] = ds.[i]?data
                        addNew s j (d (s.[j] :?> float))
                    <| rendered.Update
                )

                let streams =
                    chart.Charts
                    |> Seq.map (fun chart -> chart.DataSet)
                    |> extractStreams

                onCombinedEvent streams (Seq.length chart.Charts) window
                <| fun _ _ ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iter (fun d ->
                        popFrom (d?data : obj [])    
                    )
                    popFrom labels
                    rendered.Update()
                <| fun a (arr, label) ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iteri (fun i (d: obj) ->
                        let dd : obj[] = d?data
                        addNew (dd) (dd.Length) (arr.[i])
                    )
                    addNew labels (labels.Length) (label)
                    rendered.Update()

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
                    ChartJs.ChartData(
                        Datasets =
                            (chart.Charts
                            |> Seq.map (fun chart ->
                                let initials = mkInitial chart.DataSet window
                                ChartJs.BarChartDataSet(
                                    Label = chart.Config.Title,
                                    BackgroundColor = Union1Of2 (string chart.SeriesConfig.FillColor),
                                    BorderColor = Union1Of2 (string chart.SeriesConfig.StrokeColor),
                                    Data = (initials |> Array.map snd)) :> ChartJs.ADataSet 
                            )
                            |> Seq.toArray)
                    )

                data.Labels <- (labels |> Seq.toArray)

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.Options()

                let chartcreate =
                    ChartJs.ChartCreate(data, options)

                let rendered = 
                    ChartJs.Chart(canvas, chartcreate)

                chart.Charts
                |> Seq.iteri (fun i chart ->
                    registerUpdater (chart :> IMutableChart<float, int>)
                    <| fun (j, d) ->
                        let data : obj = rendered?data
                        let ds : obj [] = data?datasets
                        let s : obj [] = ds.[i]?data
                        addNew s j (d (s.[j] :?> float))
                    <| rendered.Update
                )

                let streams =
                    chart.Charts
                    |> Seq.map (fun chart -> chart.DataSet)
                    |> extractStreams

                onCombinedEvent streams (Seq.length chart.Charts) window
                <| fun _ _ ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iter (fun d ->
                        popFrom (d?data : obj [])    
                    )
                    popFrom labels
                    rendered.Update()
                <| fun a (arr, label) ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iteri (fun i (d: obj) ->
                        let dd : obj[] = d?data
                        addNew (dd) (dd.Length) (arr.[i])
                    )
                    addNew labels (labels.Length) (label)
                    rendered.Update()

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
                    ChartJs.ChartData(
                        Datasets =
                            (chart.Charts
                            |> Seq.map (fun chart ->
                                let initials = mkInitial chart.DataSet window
                                ChartJs.RadarChartDataSet(
                                    Label = chart.Config.Title,
                                    Fill = Union1Of2 chart.SeriesConfig.IsFilled,
                                    BackgroundColor = Union1Of2 (string chart.SeriesConfig.FillColor),
                                    BorderColor = Union1Of2 (string chart.SeriesConfig.StrokeColor),
                                    PointBackgroundColor = Union1Of2(string chart.ColorConfig.PointColor),
                                    PointHoverBackgroundColor = Union1Of2(string chart.ColorConfig.PointHighlightFill),
                                    PointHoverBorderColor = Union1Of2(string chart.ColorConfig.PointHighlightStroke),
                                    PointBorderColor = Union1Of2(string chart.ColorConfig.PointStrokeColor),
                                    Data = (initials |> Array.map snd)) :> ChartJs.ADataSet
                            )
                            |> Seq.toArray)
                    )

                data.Labels <- (labels |> Seq.toArray)

                let options =
                    defaultArg
                    <| cfg
                    <| ChartJs.RadarChartOptions()

                let chartcreate =
                    ChartJs.ChartCreate(data, options)

                let rendered = 
                    ChartJs.Chart(canvas, chartcreate)

                chart.Charts
                |> Seq.iteri (fun i chart ->
                    registerUpdater (chart :> IMutableChart<float, int>)
                    <| fun (j, d) ->
                        let data : obj = rendered?data
                        let ds : obj [] = data?datasets
                        let s : obj [] = ds.[i]?data
                        addNew s j (d (s.[j] :?> float))
                    <| rendered.Update
                )

                let streams =
                    chart.Charts
                    |> Seq.map (fun chart -> chart.DataSet)
                    |> extractStreams


                onCombinedEvent streams (Seq.length chart.Charts) window
                <| fun _ _ ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iter (fun d ->
                        popFrom (d?data : obj [])    
                    )
                    popFrom labels
                    rendered.Update()
                <| fun a (arr, label) ->
                    let data: obj = rendered?data
                    let ds : obj [] = data?datasets
                    let labels : obj [] = data?labels
                    ds
                    |> Array.iteri (fun i (d: obj) ->
                        let dd : obj[] = d?data
                        addNew (dd) (dd.Length) (arr.[i])
                    )
                    addNew labels (labels.Length) (label)
                    rendered.Update()

    type ChartJs =
        static member Render(chart : Charts.LineChart,
                             ?Size : Size,
                             ?Config : ChartJs.Options,
                             ?Window : int) =
            ChartJsInternal.RenderLineChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.BarChart,
                             ?Size : Size,
                             ?Config : ChartJs.Options,
                             ?Window : int) =
            ChartJsInternal.RenderBarChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.RadarChart,
                             ?Size : Size,
                             ?Config : ChartJs.RadarChartOptions,
                             ?Window : int) =
            ChartJsInternal.RenderRadarChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.PieChart,
                             ?Size : Size,
                             ?Config : ChartJs.PieDoughnutChartOptions,
                             ?Window : int) =
            let typ = PolarChartType.Pie <| defaultArg Config (ChartJs.PieDoughnutChartOptions())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ Window

        static member Render(chart : Charts.DoughnutChart,
                             ?Size : Size,
                             ?Config : ChartJs.PieDoughnutChartOptions,
                             ?Window : int) =
            let typ = PolarChartType.Doughnut <| defaultArg Config (ChartJs.PieDoughnutChartOptions())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ Window

        static member Render(chart : Charts.PolarAreaChart,
                             ?Size : Size,
                             ?Config : ChartJs.PolarAreaChartOptions,
                             ?Window : int) =
            let typ = PolarChartType.PolarArea <| defaultArg Config (ChartJs.PolarAreaChartOptions())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ Window

        static member Render(chart : Charts.CompositeChart<LineChart>,
                             ?Size : Size,
                             ?Config : ChartJs.Options,
                             ?Window : int) =
            ChartJsInternal.RenderCombinedLineChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.CompositeChart<BarChart>,
                             ?Size : Size,
                             ?Config : ChartJs.Options,
                             ?Window : int) =
            ChartJsInternal.RenderCombinedBarChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.CompositeChart<RadarChart>,
                             ?Size : Size,
                             ?Config : ChartJs.RadarChartOptions,
                             ?Window : int) =
            ChartJsInternal.RenderCombinedRadarChart chart (defaultArg Size defaultSize) Config Window


    open WebSharper.Plotly
    //open WebSharper.Core

    //[<Sealed>]
    //type NotSupportedMacro() =

    //    //let stringT = { Generics = []; Entity = typeof<string> } : AST.Concrete<string>

    //    inherit Macro()

    //    override __.TranslateCall(c: MacroCall) =
    //        match c.Arguments, c.Method.Entity.Value.Parameters with
    //        | [arg1; arg2], [AST.Type.ConcreteType string; AST.Type.ConcreteType x] when string.Entity.Value.FullName = "System.String" ->
    //            if arg1. arg1 then
    //                c.Compilation.AddWarning (None, "")
    //            MacroOk AST.Expression.Undefined
    //        | _ -> MacroError "Not valid usage of NotSupportedMacro"

    type Plotly =
        static member Render(chart : Charts.LineChart,
                ?Size : Size,
                ?Config : Plotly.Options,
                ?Window : int) =
            let options =
                let data =
                    match chart.DataSet with
                    | DataType.Live l -> [||]
                    | DataType.Static s -> 
                        Window
                        |> Option.fold (fun s w ->
                            let skp = s.Length - w
                            if skp >= s.Length then [||]
                            elif skp <= 0 then s
                            else s.[skp..]
                        ) (Seq.toArray s)
                let options = ScatterOptions()
                options.X <- data |> Array.map fst
                options.Y <- data |> Array.map snd
                options.Name <- chart.Config.Title
                if chart.SeriesConfig.IsFilled then
                    options.Fill <- Fill.Tozeroy
                    options.Fillcolor <- string chart.SeriesConfig.FillColor
                options.Line <-
                    ScatterLine(Color = string chart.SeriesConfig.StrokeColor)

                options.Marker <-
                    ScatterMarker(
                        Color = string chart.ColorConfig.PointColor,
                        Line = ScatterMarkerLine(Color = string chart.ColorConfig.PointStrokeColor)
                    )
                [| options |]
            withNewDiv (defaultArg Size defaultSize) <| fun el ->
                let el = el :?> HTMLElement
                let layout = Layout()
                layout.Xaxis <- LayoutXAxis(Title=LayoutAxisTitle(Text = chart.SeriesConfig.XAxis))
                layout.Yaxis <- LayoutYAxis(Title=LayoutAxisTitle(Text = chart.SeriesConfig.YAxis))
                Plotly.NewPlot(el, options, layout, defaultArg Config <| Plotly.Options()) |> ignore


        static member Render(chart : Charts.BarChart,
                 ?Size : Size,
                 ?Config : ChartJs.Options,
                 ?Window : int) =
            ChartJsInternal.RenderBarChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.RadarChart,
                 ?Size : Size,
                 ?Config : ChartJs.RadarChartOptions,
                 ?Window : int) =
            ChartJsInternal.RenderRadarChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.PieChart,
                 ?Size : Size,
                 ?Config : ChartJs.PieDoughnutChartOptions,
                 ?Window : int) =
            let typ = PolarChartType.Pie <| defaultArg Config (ChartJs.PieDoughnutChartOptions())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ Window

        static member Render(chart : Charts.DoughnutChart,
                 ?Size : Size,
                 ?Config : ChartJs.PieDoughnutChartOptions,
                 ?Window : int) =
            let typ = PolarChartType.Doughnut <| defaultArg Config (ChartJs.PieDoughnutChartOptions())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ Window

        static member Render(chart : Charts.PolarAreaChart,
                 ?Size : Size,
                 ?Config : ChartJs.PolarAreaChartOptions,
                 ?Window : int) =
            let typ = PolarChartType.PolarArea <| defaultArg Config (ChartJs.PolarAreaChartOptions())
            ChartJsInternal.RenderPolarAreaChart chart (defaultArg Size defaultSize) typ Window

        static member Render(chart : Charts.CompositeChart<LineChart>,
                 ?Size : Size,
                 ?Config : ChartJs.Options,
                 ?Window : int) =
            ChartJsInternal.RenderCombinedLineChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.CompositeChart<BarChart>,
                 ?Size : Size,
                 ?Config : ChartJs.Options,
                 ?Window : int) =
            ChartJsInternal.RenderCombinedBarChart chart (defaultArg Size defaultSize) Config Window

        static member Render(chart : Charts.CompositeChart<RadarChart>,
                 ?Size : Size,
                 ?Config : ChartJs.RadarChartOptions,
                 ?Window : int) =
            ChartJsInternal.RenderCombinedRadarChart chart (defaultArg Size defaultSize) Config Window
                
