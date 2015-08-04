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

    type GenericChartConfig =
        { Title : string
          XAxis : string
          YAxis : string
          FillColor : Color
          StrokeColor : Color
          PointColor : Color } // TODO split off specific configs

    let internal defaultChartConfig =
        { Title = "Chart"
          XAxis = "x"
          YAxis = "y"
          FillColor = Color.Rgba(220, 220, 220, 0.2)
          StrokeColor = Color.Rgba(220, 220, 220, 1.)
          PointColor = Color.Rgba(220, 220, 220, 1.) }

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
    type GenericChart<'Self when 'Self :> GenericChart<'Self>> =
        abstract member Config : GenericChartConfig

        abstract member WithTitle : string -> 'Self
        abstract member WithXAxis : string -> 'Self
        abstract member WithYAxis : string -> 'Self
        abstract member WithFillColor : Color -> 'Self
        abstract member WithStrokeColor : Color -> 'Self

    type LineChart internal (dataset : DataType<string * float>, cfg : GenericChartConfig) = 
        let cst x = (x :> GenericChart<LineChart>)

        member x.DataSet = dataset
        member x.WithPointColor color = LineChart(dataset, { cfg with PointColor = color })

        interface GenericChart<LineChart> with
            override x.Config = cfg

            override x.WithTitle(title) = LineChart(dataset, { cfg with Title = title })
            override x.WithXAxis(xAxis) = LineChart(dataset, { cfg with XAxis = xAxis })
            override x.WithYAxis(yAxis) = LineChart(dataset, { cfg with YAxis = yAxis })
            override x.WithFillColor(color) = LineChart(dataset, { cfg with FillColor = color })
            override x.WithStrokeColor(color) = LineChart(dataset, { cfg with StrokeColor = color })

        member x.Config 
            with [<Name "get__Config">] get () = (cst x).Config

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

    type BarChart internal (dataset : DataType<string * float>, cfg : GenericChartConfig) = 
        let cst x = (x :> GenericChart<BarChart>)

        member x.DataSet = dataset

        interface GenericChart<BarChart> with
            override x.Config = cfg

            override x.WithTitle(title) = BarChart(dataset, { cfg with Title = title })
            override x.WithXAxis(xAxis) = BarChart(dataset, { cfg with XAxis = xAxis })
            override x.WithYAxis(yAxis) = BarChart(dataset, { cfg with YAxis = yAxis })
            override x.WithFillColor(color) = BarChart(dataset, { cfg with FillColor = color })
            override x.WithStrokeColor(color) = BarChart(dataset, { cfg with StrokeColor = color })

        member x.Config 
            with [<Name "get__Config">] get () = (cst x).Config

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

    type RadarChart internal (dataset : DataType<string * float>, cfg : GenericChartConfig) = 
        let cst x = (x :> GenericChart<RadarChart>)

        member x.DataSet = dataset

        interface GenericChart<RadarChart> with
            override x.Config = cfg

            override x.WithTitle(title) = RadarChart(dataset, { cfg with Title = title })
            override x.WithXAxis(xAxis) = RadarChart(dataset, { cfg with XAxis = xAxis })
            override x.WithYAxis(yAxis) = RadarChart(dataset, { cfg with YAxis = yAxis })
            override x.WithFillColor(color) = RadarChart(dataset, { cfg with FillColor = color })
            override x.WithStrokeColor(color) = RadarChart(dataset, { cfg with StrokeColor = color })

        member x.Config 
            with [<Name "get__Config">] get () = (cst x).Config

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

    // TODO most of the config doesn't make sense here
    [<Interface>]
    type GenericPolarAreaChart<'Self when 'Self :> GenericPolarAreaChart<'Self>> =
        inherit GenericChart<'Self>
        
        abstract member DataSet : DataType<PolarData>

    type PolarAreaChart internal (dataset : DataType<PolarData>, cfg : GenericChartConfig) =
        let cst x = (x :> GenericPolarAreaChart<PolarAreaChart>)

        interface GenericPolarAreaChart<PolarAreaChart> with
            override x.Config = cfg
            override x.DataSet = dataset

            override x.WithTitle(title) = PolarAreaChart(dataset, { cfg with Title = title })
            override x.WithXAxis(xAxis) = PolarAreaChart(dataset, { cfg with XAxis = xAxis })
            override x.WithYAxis(yAxis) = PolarAreaChart(dataset, { cfg with YAxis = yAxis })
            override x.WithFillColor(color) = PolarAreaChart(dataset, { cfg with FillColor = color })
            override x.WithStrokeColor(color) = PolarAreaChart(dataset, { cfg with StrokeColor = color })

        member x.Config 
            with [<Name "get__Config">] get () = (cst x).Config

        member x.DataSet 
            with [<Name "__DataSet">] get () = (cst x).DataSet

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

    type PieChart internal (dataset : DataType<PolarData>, cfg : GenericChartConfig) =
        let cst x = (x :> GenericPolarAreaChart<PieChart>)

        interface GenericPolarAreaChart<PieChart> with
            override x.Config = cfg
            override x.DataSet = dataset

            override x.WithTitle(title) = PieChart(dataset, { cfg with Title = title })
            override x.WithXAxis(xAxis) = PieChart(dataset, { cfg with XAxis = xAxis })
            override x.WithYAxis(yAxis) = PieChart(dataset, { cfg with YAxis = yAxis })
            override x.WithFillColor(color) = PieChart(dataset, { cfg with FillColor = color })
            override x.WithStrokeColor(color) = PieChart(dataset, { cfg with StrokeColor = color })

        member x.Config 
            with [<Name "get__Config">] get () = (cst x).Config

        member x.DataSet 
            with [<Name "get__DataSet">] get () = (cst x).DataSet

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
            

    type DoughnutChart internal (dataset : DataType<PolarData>, cfg : GenericChartConfig) =
        let cst x = (x :> GenericPolarAreaChart<DoughnutChart>)

        interface GenericPolarAreaChart<DoughnutChart> with
            override x.Config = cfg
            override x.DataSet = dataset

            override x.WithTitle(title) = DoughnutChart(dataset, { cfg with Title = title })
            override x.WithXAxis(xAxis) = DoughnutChart(dataset, { cfg with XAxis = xAxis })
            override x.WithYAxis(yAxis) = DoughnutChart(dataset, { cfg with YAxis = yAxis })
            override x.WithFillColor(color) = DoughnutChart(dataset, { cfg with FillColor = color })
            override x.WithStrokeColor(color) = DoughnutChart(dataset, { cfg with StrokeColor = color })

        member x.Config 
            with [<Name "get__Config">] get () = (cst x).Config

        member x.DataSet
            with [<Name "get__DataSet">] get () = (cst x).DataSet

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

    type CompisiteChart<'T when 'T :> GenericChart<'T>> internal 
        (charts : seq<'T>) =
            
        member x.Charts = charts

[<JavaScript>]
type Chart =
    static member Line(dataset) =
        Charts.LineChart(Charts.Static dataset, Charts.defaultChartConfig)

    static member Bar(dataset) =
        Charts.BarChart(Charts.Static dataset, Charts.defaultChartConfig)

    static member Radar(dataset) =
        Charts.RadarChart(Charts.Static dataset, Charts.defaultChartConfig)

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
        Charts.CompisiteChart(charts)

[<JavaScript>]
type LiveChart =
    static member Line(dataset) =
        Charts.LineChart(Charts.Live dataset, Charts.defaultChartConfig)

    static member Line(dataset) =
        let s =
            Reactive.Select
            <| Reactive.Aggregate dataset (0, 0.0) (fun (s, _) c -> (s + 1, c)) 
            <| fun (a, b) -> (string a, b)
        Charts.LineChart(Charts.Live s, Charts.defaultChartConfig)

    static member Bar(dataset) =
        Charts.BarChart(Charts.Live dataset, Charts.defaultChartConfig)

    static member Bar(dataset) =
        let s =
            Reactive.Select
            <| Reactive.Aggregate dataset (0, 0.0) (fun (s, _) c -> (s + 1, c)) 
            <| fun (a, b) -> (string a, b)
        Charts.BarChart(Charts.Live s, Charts.defaultChartConfig)
