namespace WebSharper.Charting

open System
open WebSharper
open IntelliFactory.Reactive

[<JavaScript>]
module Charts =
    type GenericChartConfig =
        { Title : string
          XAxis : string
          YAxis : string
          FillColor : Color
          StrokeColor : Color
          PointColor : Color }

    let internal defaultChartConfig =
        { Title = "Chart"
          XAxis = "x"
          YAxis = "y"
          FillColor = Color.Rgba(220, 220, 220, 0.2)
          StrokeColor = Color.Rgba(220, 220, 220, 1.)
          PointColor = Color.Rgba(220, 220, 220, 1.) }

    [<Interface>]
    type GenericChart<'Self when 'Self :> GenericChart<'Self>> =
        abstract member Config : GenericChartConfig

        abstract member WithTitle : string -> 'Self
        abstract member WithXAxis : string -> 'Self
        abstract member WithYAxis : string -> 'Self
        abstract member WithFillColor : Color -> 'Self
        abstract member WithStrokeColor : Color -> 'Self
        abstract member WithPointColor : Color -> 'Self

    type LineChart internal (dataset : seq<string * float>, cfg : GenericChartConfig) = 
        let cst x = (x :> GenericChart<LineChart>)

        member x.DataSet = dataset

        interface GenericChart<LineChart> with
            override x.Config = cfg

            override x.WithTitle(title) = LineChart(dataset, { cfg with Title = title })
            override x.WithXAxis(xAxis) = LineChart(dataset, { cfg with XAxis = xAxis })
            override x.WithYAxis(yAxis) = LineChart(dataset, { cfg with YAxis = yAxis })
            override x.WithFillColor(color) = LineChart(dataset, { cfg with FillColor = color })
            override x.WithStrokeColor(color) = LineChart(dataset, { cfg with StrokeColor = color })
            override x.WithPointColor(color) = LineChart(dataset, { cfg with PointColor = color })

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

        [<Name "__WithPointColor">]
        member x.WithPointColor color = (cst x).WithPointColor(color)

    type LiveLineChart internal (dataset : IObservable<string * float>, cfg : GenericChartConfig) =
        let cst x = (x :> GenericChart<LiveLineChart>)

        member x.DataSet = dataset

        interface GenericChart<LiveLineChart> with
            override x.Config = cfg

            override x.WithTitle(title) = LiveLineChart(dataset, { cfg with Title = title })
            override x.WithXAxis(xAxis) = LiveLineChart(dataset, { cfg with XAxis = xAxis })
            override x.WithYAxis(yAxis) = LiveLineChart(dataset, { cfg with YAxis = yAxis })
            override x.WithFillColor(color) = LiveLineChart(dataset, { cfg with FillColor = color })
            override x.WithStrokeColor(color) = LiveLineChart(dataset, { cfg with StrokeColor = color })
            override x.WithPointColor(color) = LiveLineChart(dataset, { cfg with PointColor = color })

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

        [<Name "__WithPointColor">]
        member x.WithPointColor color = (cst x).WithPointColor(color)

//    type CompisiteChart<'Data, 'S when 'S :> GenericChart<'Data, 'S>> internal 
//        (charts : seq<GenericChart<'Data, 'S>>) =
//            
//        member x.DataSets = charts |> Seq.map (fun c -> c.DataSet)
//        member x.Titles = charts |> Seq.map (fun c -> c.Config.Title)

[<JavaScript>]
type Chart =
    static member Line(dataset) =
        Charts.LineChart(dataset, Charts.defaultChartConfig)

//    static member Combine(charts : seq<Charts.GenericChart<'T, 'S>>) =
//        Charts.CompisiteChart(charts)

[<JavaScript>]
type LiveChart =
    static member Line(dataset) =
        Charts.LiveLineChart(dataset, Charts.defaultChartConfig)

    static member Line(dataset) =
        let s =
            Reactive.Select
            <| Reactive.Aggregate dataset (0, 0.0) (fun (s, _) c -> (s + 1, c)) 
            <| fun (a, b) -> (string a, b)
        Charts.LiveLineChart(s, Charts.defaultChartConfig)
