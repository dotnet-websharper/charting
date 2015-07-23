namespace WebSharper.Charting

module Sandbox =
    open WebSharper
    open System

    type Size = { Width : int; Height : int }

    [<JavaScript>]
    module Charts =
        [<Interface>]
        type GenericChart<'Self when 'Self :> GenericChart<'Self>> =
            abstract member Title : string
            abstract member WithTitle : string -> 'Self
                

        type LineChart internal (dataset : seq<string * float>, ?title : string) = 
            member x.DataSet = dataset

            interface GenericChart<LineChart> with
                override x.Title = defaultArg title "Line Chart"
                override x.WithTitle(title) =
                    LineChart(dataset, title)

            [<Name "__WithTitle">]
            member x.WithTitle title =
                (x :> GenericChart<LineChart>).WithTitle(title)

        type LiveLineChart internal (dataset : IObservable<string * float>, ?title : string) =
            member x.DataSet = dataset

            interface GenericChart<LiveLineChart> with
                override x.Title = defaultArg title "Line Chart"
                override x.WithTitle(title) =
                    LiveLineChart(dataset, title)

            [<Name "__WithTitle">]
            member x.WithTitle title =
                (x :> GenericChart<LiveLineChart>).WithTitle(title)

    [<JavaScript>]
    module Renderers =
        open Charts
        open WebSharper.JavaScript
        open WebSharper.Html.Client

        [<Interface>]
        type IRenderer<'T when 'T :> GenericChart<'T>> =
            abstract member Render : 'T -> Element

        module ChartJs =
            open WebSharper.ChartJs

            let private withNewCanvas size k =
                Canvas [ Attr.Width <| string size.Width; Attr.Height <| string size.Height ]
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
                                            Data = (chart.DataSet |> Seq.map snd |> Seq.toArray)) |])

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
                                            [| ChartJs.LineChartDataset(Data = [||]) |])

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

    [<JavaScript>]
    type Chart =
        static member Line(dataset) =
            Charts.LineChart(dataset)

    open IntelliFactory.Reactive

    [<JavaScript>]
    type LiveChart =
        static member Line(dataset) =
            Charts.LiveLineChart(dataset)

        static member Line(dataset) =
            let s =
                Reactive.Select
                <| Reactive.Aggregate dataset (0, 0.0) (fun (s, _) c -> (s + 1, c)) 
                <| fun (a, b) -> (string a, b)
            Charts.LiveLineChart(s)
