namespace WebSharper.Charting

open System
open WebSharper
open IntelliFactory.Reactive

[<JavaScript>]
module Charts =
    type DataType<'T> =
        | Static of seq<'T>
        | Live of IObservable<'T>

    module DataType =
        let Map fn dt =
            match dt with
            | Static s -> Static <| Seq.map fn s
            | Live io -> Live <| Reactive.Select io fn

    type ChartConfig = { Title : string }
    let internal defaultChartConfig = { Title = "Chart" }

    type SeriesChartConfig =
        { XAxis : string
          YAxis : string
          FillColor : Color
          StrokeColor : Color }
    let internal defaultSeriesChartConfig =
        { XAxis = "x"
          YAxis = "y"
          FillColor = Color.Rgba(220, 220, 220, 0.2)
          StrokeColor = Color.Rgba(220, 220, 220, 1.) }

    type ColorConfig = 
        { PointColor : Color
          PointHighlightFill : Color
          PointHighlightStroke : Color
          PointStrokeColor : Color }
    let internal defaultColorConfig = 
        { PointColor = Color.Rgba(220, 220, 220, 1.)
          PointHighlightFill = Color.Hex "#fff"
          PointHighlightStroke = Color.Rgba(220, 220, 220, 1.)
          PointStrokeColor = Color.Hex "#fff" }

    type PolarData =
        { Value : float 
          Color : Color 
          Highlight : Color
          Label : string }

    let internal defaultPolarData =
        let rand = System.Random()
        fun label data ->
            let color, highlight =
                let r = rand.Next(0, 256)
                let g = rand.Next(0, 256)
                let b = rand.Next(0, 256)
                Color.Rgba(r, g, b, 1.), Color.Rgba(r, g, b, 0.6)
            { Value = data
              Color = color
              Highlight = highlight
              Label = label }

    [<Interface>]
    type IChart<'Self when 'Self :> IChart<'Self>> =
        abstract member Config : ChartConfig

        abstract member WithTitle : string -> 'Self

    [<Interface>]
    type IColorChart<'Self when 'Self :> IColorChart<'Self>> =
        inherit IChart<'Self>

        abstract member ColorConfig : ColorConfig

        abstract member WithPointColor : Color -> 'Self
        abstract member WithPointHighlightFill : Color -> 'Self
        abstract member WithPointHighlightStroke : Color -> 'Self
        abstract member WithPointStrokeColor : Color -> 'Self

    [<Interface>]
    type ISeriesChart<'Self when 'Self :> ISeriesChart<'Self>> =
        inherit IChart<'Self>

        abstract member SeriesConfig : SeriesChartConfig

        abstract member WithXAxis : string -> 'Self
        abstract member WithYAxis : string -> 'Self
        abstract member WithFillColor : Color -> 'Self
        abstract member WithStrokeColor : Color -> 'Self

    [<Interface>]
    type IMutableChart<'T, 'U> =
        abstract member UpdateData : props : 'U * update : ('T -> 'T) -> unit
        abstract member OnUpdate : ('U * ('T -> 'T) -> unit) -> unit

    type LineChart internal (dataset : DataType<string * float>, cfg : ChartConfig,
                             scfg : SeriesChartConfig, ccfg : ColorConfig) = 
        
        let event = Event<int * (float -> float)>()

        interface IMutableChart<float, int> with
            override x.UpdateData(data, props) =
                event.Trigger((data, props))

            override x.OnUpdate(fn) =
                event.Publish.Add fn

        [<Name "__UpdateData">]
        member x.UpdateData(data, props) = (x :> IMutableChart<float, int>).UpdateData(data, props)

        member x.DataSet = dataset

        interface IChart<LineChart> with
            override x.Config = cfg

            override x.WithTitle(title) = LineChart(dataset, { cfg with Title = title }, scfg, ccfg)

        interface ISeriesChart<LineChart> with
            override x.SeriesConfig = scfg

            override x.WithXAxis(xAxis) = LineChart(dataset, cfg, { scfg with XAxis = xAxis }, ccfg)
            override x.WithYAxis(yAxis) = LineChart(dataset, cfg, { scfg with YAxis = yAxis }, ccfg)
            override x.WithFillColor(color) = LineChart(dataset, cfg, { scfg with FillColor = color }, ccfg)
            override x.WithStrokeColor(color) = LineChart(dataset, cfg, { scfg with StrokeColor = color }, ccfg)

        interface IColorChart<LineChart> with
            override x.ColorConfig = ccfg

            override x.WithPointColor color = 
                LineChart(dataset, cfg, scfg, { ccfg with PointColor = color })
            override x.WithPointHighlightFill color = 
                LineChart(dataset, cfg, scfg, { ccfg with PointHighlightFill = color })
            override x.WithPointHighlightStroke color = 
                LineChart(dataset, cfg, scfg, { ccfg with PointHighlightStroke = color })
            override x.WithPointStrokeColor color = 
                LineChart(dataset, cfg, scfg, { ccfg with PointStrokeColor = color })

        // IChart
        member x.Config 
            with [<Name "get__Config">] get () = (x :> IChart<LineChart>).Config

        [<Name "__WithTitle">]
        member x.WithTitle title = (x :> IChart<LineChart>).WithTitle(title)

        // ISeriesChart
        member x.SeriesConfig
            with [<Name "get__SeriesConfig">] get () = (x :> ISeriesChart<LineChart>).SeriesConfig

        [<Name "__WithXAxis">]
        member x.WithXAxis xAxis = (x :> ISeriesChart<LineChart>).WithXAxis(xAxis)

        [<Name "__WithYAxis">]
        member x.WithYAxis yAxis = (x :> ISeriesChart<LineChart>).WithYAxis(yAxis)

        [<Name "__WithFillColor">]
        member x.WithFillColor color = (x :> ISeriesChart<LineChart>).WithFillColor(color)

        [<Name "__WithStrokeColor">]
        member x.WithStrokeColor color = (x :> ISeriesChart<LineChart>).WithStrokeColor(color)

        // IColorChart
        member x.ColorConfig
            with [<Name "get__ColorConfig">] get () = (x :> IColorChart<LineChart>).ColorConfig

        [<Name "__WithPointColor">]
        member x.WithPointColor color = (x :> IColorChart<LineChart>).WithPointColor(color)

        [<Name "__WithPointHighlightFill">]
        member x.WithPointHighlightFill color = (x :> IColorChart<LineChart>).WithPointHighlightFill(color)

        [<Name "WithPointHighlightStroke">]
        member x.WithPointHighlightStroke color = (x :> IColorChart<LineChart>).WithPointHighlightStroke(color)

        [<Name "__WithPointStrokeColor">]
        member x.WithPointStrokeColor color = (x :> IColorChart<LineChart>).WithPointStrokeColor(color)

    type BarChart internal (dataset : DataType<string * float>, cfg : ChartConfig, scfg : SeriesChartConfig) = 
        let cst x = (x :> ISeriesChart<BarChart>)

        let event = Event<int * (float -> float)>()

        interface IMutableChart<float, int> with
            override x.UpdateData(data, props) =
                event.Trigger((data, props))

            override x.OnUpdate(fn) =
                event.Publish.Add fn

        [<Name "__UpdateData">]
        member x.UpdateData(data, props) = (x :> IMutableChart<float, int>).UpdateData(data, props)

        member x.DataSet = dataset

        interface ISeriesChart<BarChart> with
            override x.Config = cfg
            override x.SeriesConfig = scfg

            override x.WithTitle(title) = BarChart(dataset, { cfg with Title = title }, scfg)
            override x.WithXAxis(xAxis) = BarChart(dataset, cfg, { scfg with XAxis = xAxis })
            override x.WithYAxis(yAxis) = BarChart(dataset, cfg, { scfg with YAxis = yAxis })
            override x.WithFillColor(color) = BarChart(dataset, cfg, { scfg with FillColor = color })
            override x.WithStrokeColor(color) = BarChart(dataset, cfg, { scfg with StrokeColor = color })

        member x.Config 
            with [<Name "get__Config">] get () = (cst x).Config

        member x.SeriesConfig 
            with [<Name "get__SeriesConfig">] get () = (cst x).SeriesConfig

        [<Name "__WithTitle">]
        member x.WithTitle title = (cst x).WithTitle(title)

        [<Name "__WithXAxis">]
        member x.WithXAxis xAxis = (cst x).WithXAxis(xAxis)

        [<Name "__WithYAxis">]
        member x.WithYAxis yAxis = (cst x).WithYAxis(yAxis)

        [<Name "__WithFillColor">]
        member x.WithFillColor color = (cst x).WithFillColor(color)

        [<Name "__WithStrokeColor">]
        member x.WithStrokeColor color = (cst x).WithStrokeColor(color)

    type RadarChart internal (dataset : DataType<string * float>, cfg : ChartConfig,
                              scfg : SeriesChartConfig, ccfg : ColorConfig) = 

        let event = Event<int * (float -> float)>()

        interface IMutableChart<float, int> with
            override x.UpdateData(data, props) =
                event.Trigger((data, props))

            override x.OnUpdate(fn) =
                event.Publish.Add fn

        [<Name "__UpdateData">]
        member x.UpdateData(data, props) = (x :> IMutableChart<float, int>).UpdateData(data, props)

        member x.DataSet = dataset

        interface IChart<RadarChart> with
            override x.Config = cfg

            override x.WithTitle(title) = RadarChart(dataset, { cfg with Title = title }, scfg, ccfg)

        interface ISeriesChart<RadarChart> with
            override x.SeriesConfig = scfg

            override x.WithXAxis(xAxis) = RadarChart(dataset, cfg, { scfg with XAxis = xAxis }, ccfg)
            override x.WithYAxis(yAxis) = RadarChart(dataset, cfg, { scfg with YAxis = yAxis }, ccfg)
            override x.WithFillColor(color) = RadarChart(dataset, cfg, { scfg with FillColor = color }, ccfg)
            override x.WithStrokeColor(color) = RadarChart(dataset, cfg, { scfg with StrokeColor = color }, ccfg)

        interface IColorChart<RadarChart> with
            override x.ColorConfig = ccfg

            override x.WithPointColor color = 
                RadarChart(dataset, cfg, scfg, { ccfg with PointColor = color })
            override x.WithPointHighlightFill color = 
                RadarChart(dataset, cfg, scfg, { ccfg with PointHighlightFill = color })
            override x.WithPointHighlightStroke color = 
                RadarChart(dataset, cfg, scfg, { ccfg with PointHighlightStroke = color })
            override x.WithPointStrokeColor color = 
                RadarChart(dataset, cfg, scfg, { ccfg with PointStrokeColor = color })

        // IChart
        member x.Config 
            with [<Name "get__Config">] get () = (x :> IChart<RadarChart>).Config

        [<Name "__WithTitle">]
        member x.WithTitle title = (x :> IChart<RadarChart>).WithTitle(title)

        // ISeriesChart
        member x.SeriesConfig
            with [<Name "get__SeriesConfig">] get () = (x :> ISeriesChart<RadarChart>).SeriesConfig

        [<Name "__WithXAxis">]
        member x.WithXAxis xAxis = (x :> ISeriesChart<RadarChart>).WithXAxis(xAxis)

        [<Name "__WithYAxis">]
        member x.WithYAxis yAxis = (x :> ISeriesChart<RadarChart>).WithYAxis(yAxis)

        [<Name "__WithFillColor">]
        member x.WithFillColor color = (x :> ISeriesChart<RadarChart>).WithFillColor(color)

        [<Name "__WithStrokeColor">]
        member x.WithStrokeColor color = (x :> ISeriesChart<RadarChart>).WithStrokeColor(color)

        // IColorChart
        member x.ColorConfig
            with [<Name "get__ColorConfig">] get () = (x :> IColorChart<RadarChart>).ColorConfig

        [<Name "__WithPointColor">]
        member x.WithPointColor color = (x :> IColorChart<RadarChart>).WithPointColor(color)

        [<Name "__WithPointHighlightFill">]
        member x.WithPointHighlightFill color = (x :> IColorChart<RadarChart>).WithPointHighlightFill(color)

        [<Name "WithPointHighlightStroke">]
        member x.WithPointHighlightStroke color = (x :> IColorChart<RadarChart>).WithPointHighlightStroke(color)

        [<Name "__WithPointStrokeColor">]
        member x.WithPointStrokeColor color = (x :> IColorChart<RadarChart>).WithPointStrokeColor(color)

    // TODO most of the config doesn't make sense here
    [<Interface>]
    type IPolarAreaChart<'Self when 'Self :> IPolarAreaChart<'Self>> =
        inherit IMutableChart<float, int>
        inherit IChart<'Self>
        
        abstract member DataSet : DataType<PolarData>

    type PolarAreaChart internal (dataset : DataType<PolarData>, cfg : ChartConfig) =
        let cst x = (x :> IPolarAreaChart<PolarAreaChart>)

        let event = Event<int * (float -> float)>()

        interface IMutableChart<float, int> with
            override x.UpdateData(props, data) =
                event.Trigger((props, data))

            override x.OnUpdate(fn) =
                event.Publish.Add fn

        [<Name "__UpdateData">]
        member x.UpdateData(props, data) = (x :> IMutableChart<float, int>).UpdateData(props, data)

        interface IPolarAreaChart<PolarAreaChart> with
            override x.Config = cfg
            override x.DataSet = dataset

            override x.WithTitle(title) = PolarAreaChart(dataset, { cfg with Title = title })

        member x.Config 
            with [<Name "get__Config">] get () = (cst x).Config

        member x.DataSet 
            with [<Name "__DataSet">] get () = (cst x).DataSet

        [<Name "__WithTitle">]
        member x.WithTitle title = (cst x).WithTitle(title)

    type PieChart internal (dataset : DataType<PolarData>, cfg : ChartConfig) =
        let cst x = (x :> IPolarAreaChart<PieChart>)

        let event = Event<int * (float -> float)>()

        interface IMutableChart<float, int> with
            override x.UpdateData(data, props) =
                event.Trigger((data, props))

            override x.OnUpdate(fn) =
                event.Publish.Add fn

        [<Name "__UpdateData">]
        member x.UpdateData(props, data) = (x :> IMutableChart<float, int>).UpdateData(props, data)

        interface IPolarAreaChart<PieChart> with
            override x.Config = cfg
            override x.DataSet = dataset

            override x.WithTitle(title) = PieChart(dataset, { cfg with Title = title })

        member x.Config 
            with [<Name "get__Config">] get () = (cst x).Config

        member x.DataSet 
            with [<Name "get__DataSet">] get () = (cst x).DataSet

        [<Name "__WithTitle">]
        member x.WithTitle title = (cst x).WithTitle(title)

    type DoughnutChart internal (dataset : DataType<PolarData>, cfg : ChartConfig) =
        let cst x = (x :> IPolarAreaChart<DoughnutChart>)

        let event = Event<int * (float -> float)>()

        interface IMutableChart<float, int> with
            override x.UpdateData(data, props) =
                event.Trigger((data, props))

            override x.OnUpdate(fn) =
                event.Publish.Add fn

        [<Name "__UpdateData">]
        member x.UpdateData(props, data) = (x :> IMutableChart<float, int>).UpdateData(props, data)

        interface IPolarAreaChart<DoughnutChart> with
            override x.Config = cfg
            override x.DataSet = dataset

            override x.WithTitle(title) = DoughnutChart(dataset, { cfg with Title = title })

        member x.Config 
            with [<Name "get__Config">] get () = (cst x).Config

        member x.DataSet
            with [<Name "get__DataSet">] get () = (cst x).DataSet

        [<Name "__WithTitle">]
        member x.WithTitle title = (cst x).WithTitle(title)

    type CompositeChart<'T when 'T :> ISeriesChart<'T>> internal 
        (charts : seq<'T>) =
            
        member x.Charts = charts

[<JavaScript>]
type Chart =
    static member Line(dataset) =
        Charts.LineChart(Charts.Static dataset, Charts.defaultChartConfig,
                         Charts.defaultSeriesChartConfig, Charts.defaultColorConfig)

    static member Line(dataset) =
        Charts.LineChart(Charts.Static <| withIndex dataset, Charts.defaultChartConfig,
                         Charts.defaultSeriesChartConfig, Charts.defaultColorConfig)

    static member Bar(dataset) =
        Charts.BarChart(Charts.Static dataset, Charts.defaultChartConfig, Charts.defaultSeriesChartConfig)

    static member Bar(dataset) =
        Charts.BarChart(Charts.Static <| withIndex dataset, Charts.defaultChartConfig, Charts.defaultSeriesChartConfig)

    static member Radar(dataset) =
        Charts.RadarChart(Charts.Static dataset, Charts.defaultChartConfig,
                          Charts.defaultSeriesChartConfig, Charts.defaultColorConfig)

    static member Radar(dataset) =
        Charts.RadarChart(Charts.Static <| withIndex dataset, Charts.defaultChartConfig,
                          Charts.defaultSeriesChartConfig, Charts.defaultColorConfig)

    static member PolarArea(dataset) =
        Charts.PolarAreaChart(Charts.Static dataset, Charts.defaultChartConfig)

    static member PolarArea(dataset : seq<string * float>) =
        let d = dataset |> Seq.map (fun (l, v) -> Charts.defaultPolarData l v)
        Charts.PolarAreaChart(Charts.Static d, Charts.defaultChartConfig)

    static member Pie(dataset) =
        Charts.PieChart(Charts.Static dataset, Charts.defaultChartConfig)

    static member Pie(dataset : seq<string * float>) =
        let d = dataset |> Seq.map (fun (l, v) -> Charts.defaultPolarData l v)
        Charts.PieChart(Charts.Static d, Charts.defaultChartConfig)

    static member Doughnut(dataset) =
        Charts.DoughnutChart(Charts.Static dataset, Charts.defaultChartConfig)

    static member Doughnut(dataset : seq<string * float>) =
        let d = dataset |> Seq.map (fun (l, v) -> Charts.defaultPolarData l v)
        Charts.DoughnutChart(Charts.Static d, Charts.defaultChartConfig)

    static member Combine(charts) =
        Charts.CompositeChart(charts)

[<JavaScript>]
type LiveChart =
    static member Line(dataset) =
        Charts.LineChart(Charts.Live dataset, Charts.defaultChartConfig,
                         Charts.defaultSeriesChartConfig, Charts.defaultColorConfig)

    static member Line(dataset) =
        Charts.LineChart(Charts.Live <| streamWithLabel dataset, Charts.defaultChartConfig,
                         Charts.defaultSeriesChartConfig, Charts.defaultColorConfig)

    static member Bar(dataset) =
        Charts.BarChart(Charts.Live dataset, Charts.defaultChartConfig, Charts.defaultSeriesChartConfig)

    static member Bar(dataset) =
        Charts.BarChart(Charts.Live <| streamWithLabel dataset, Charts.defaultChartConfig, Charts.defaultSeriesChartConfig)

    static member Radar(dataset) =
        Charts.RadarChart(Charts.Live dataset, Charts.defaultChartConfig,
                          Charts.defaultSeriesChartConfig, Charts.defaultColorConfig)

    static member Radar(dataset) =
        Charts.RadarChart(Charts.Live <| streamWithLabel dataset, Charts.defaultChartConfig,
                          Charts.defaultSeriesChartConfig, Charts.defaultColorConfig)

    static member PolarArea(dataset) =
        Charts.PolarAreaChart(Charts.Live dataset, Charts.defaultChartConfig)

    static member PolarArea(dataset : IObservable<string * float>) =
        let d = Reactive.Select dataset (fun (l, v) -> Charts.defaultPolarData l v)
        Charts.PolarAreaChart(Charts.Live d, Charts.defaultChartConfig)

    static member Pie(dataset) =
        Charts.PieChart(Charts.Live dataset, Charts.defaultChartConfig)

    static member Pie(dataset : IObservable<string * float>) =
        let d = Reactive.Select dataset (fun (l, v) -> Charts.defaultPolarData l v)
        Charts.PieChart(Charts.Live d, Charts.defaultChartConfig)

    static member Doughnut(dataset) =
        Charts.DoughnutChart(Charts.Live dataset, Charts.defaultChartConfig)

    static member Doughnut(dataset : IObservable<string * float>) =
        let d = Reactive.Select dataset (fun (l, v) -> Charts.defaultPolarData l v)
        Charts.DoughnutChart(Charts.Live d, Charts.defaultChartConfig)
