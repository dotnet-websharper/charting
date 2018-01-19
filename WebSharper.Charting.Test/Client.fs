namespace WebSharper.Charting.Test

open WebSharper
open WebSharper.JavaScript
open WebSharper.Charting
open IntelliFactory.Reactive
open WebSharper.UI.Html
open WebSharper.UI.Client

[<Interface>]
type IChart<'Self when 'Self :> IChart<'Self>> =
    abstract member WithTitle : string -> 'Self

type EmptyChart(title : string) =
    interface IChart<EmptyChart> with
        override x.WithTitle t = EmptyChart(t)

[<JavaScript>]
module Client =
    open WebSharper.UI

    [<SPAEntryPoint>]
    let Main() =

        let ch1 = 
            Chart.Line([ for x in 1.0 .. 11.0 -> (string x, x) ])
                .WithFillColor(Color.Rgba(32, 64, 128, 1.))

        let ch2 =
            Chart.Line([ for x in 1.0 .. 10.0 -> (string x, x * x) ])
                .WithFillColor(Color.Rgba(128, 64, 32, 0.1))

        let cr =
            [
                ch1
                ch2
            ]
            |> Chart.Combine
            |> fun e -> 
                let c = ChartJs.CommonChartConfig()
                Renderers.ChartJs.Render(e, Config = c)

        let pie =
            let data =
                [
                    {
                        Charts.PolarData.Value = 25.
                        Charts.PolarData.Color = Color.Name "red"
                        Charts.PolarData.Highlight = Color.Name "orange"
                        Charts.PolarData.Label = "Part1"
                    }
                    {
                        Charts.PolarData.Value = 75.
                        Charts.PolarData.Color = Color.Name "blue"
                        Charts.PolarData.Highlight = Color.Name "green"
                        Charts.PolarData.Label = "Part2"
                    }
                ]
            Chart.Pie(data)
                .WithTitle "Pie example"
        
        let renderedPie =
            let c = ChartJs.CommonChartConfig()
            Renderers.ChartJs.Render(pie, Config = c)

        let doughnut =
            let data =
                [
                    {
                        Charts.PolarData.Value = 27.
                        Charts.PolarData.Color = Color.Name "red"
                        Charts.PolarData.Highlight = Color.Name "orange"
                        Charts.PolarData.Label = "Part1"
                    }
                    {
                        Charts.PolarData.Value = 23.
                        Charts.PolarData.Color = Color.Name "blue"
                        Charts.PolarData.Highlight = Color.Name "green"
                        Charts.PolarData.Label = "Part2"
                    }
                    {
                        Charts.PolarData.Value = 50.
                        Charts.PolarData.Color = Color.Name "yellow"
                        Charts.PolarData.Highlight = Color.Name "purple"
                        Charts.PolarData.Label = "Part3"
                    }
                ]
            Chart.Doughnut(data)
                
        
        let renderedDoughnut =
            let c = ChartJs.CommonChartConfig()
            Renderers.ChartJs.Render(doughnut, Config = c)

        let polar = 
            let data =
                [
                    {
                        Charts.PolarData.Value = 27.
                        Charts.PolarData.Color = Color.Name "red"
                        Charts.PolarData.Highlight = Color.Name "orange"
                        Charts.PolarData.Label = "Part1"
                    }
                    {
                        Charts.PolarData.Value = 23.
                        Charts.PolarData.Color = Color.Name "blue"
                        Charts.PolarData.Highlight = Color.Name "green"
                        Charts.PolarData.Label = "Part2"
                    }
                    {
                        Charts.PolarData.Value = 37.
                        Charts.PolarData.Color = Color.Name "yellow"
                        Charts.PolarData.Highlight = Color.Name "purple"
                        Charts.PolarData.Label = "Part3"
                    }
                    {
                        Charts.PolarData.Value = 13.
                        Charts.PolarData.Color = Color.Name "brown"
                        Charts.PolarData.Highlight = Color.Name "black"
                        Charts.PolarData.Label = "Part4"
                    }
                ]
            Chart.PolarArea(data)

        let renderedPolar =
            let c = ChartJs.CommonChartConfig()
            Renderers.ChartJs.Render(polar, Config = c)

        let stream1 = Event<string * float>()
        let stream2 = Event<string * float>()


        let b1 = LiveChart.Radar stream1.Publish
        let b2 =
            LiveChart.Radar(stream2.Publish)
                .WithStrokeColor(Color.Name "red")
                .WithFillColor(Color.Rgba(100, 160, 100, 0.1))

        let crs =
            [
                b1
                b2
            ]
            |> Chart.Combine
            |> fun ch -> Renderers.ChartJs.Render(ch, Window = 10)

        let bar1 =
            Chart.Bar([ for x in 1.0 .. 10.0 -> (string x, x) ])
                .WithFillColor(Color.Name "red")

        let bar2 =
            Chart.Bar([ for x in 1.0 .. 10.0 -> (string x, x * x ) ])
                .WithFillColor(Color.Name "blue")

        let combinedBar =
            [
                bar1
                bar2
            ]
            |> Chart.Combine
            |> fun ch -> Renderers.ChartJs.Render(ch)

        let rnd = System.Random()
        async {
            while true do
                do! Async.Sleep 300
                let r = rnd.NextDouble() * 100.
                try
                    b1.UpdateData(2, fun _ -> r)
                    b2.UpdateData(2, fun _ -> r / 2.)
                with _ ->
                    ()
        }
        |> Async.Start

        let generate (s : Event<string * float>) range miniv maxiv =
            let rand = System.Random()
            let mutable x = 1
            async {
                while true do
                    let iv = rand.Next(miniv, maxiv)
                    do! Async.Sleep iv
                    let a = rand.NextDouble() * range
                    let st = string x
                    x <- x + 1
                    s.Trigger (st, a)
            }
            |> Async.Start

        let stream3 = Event<float>()

        let generateWithoutLabel (s : Event<float>) range miniv maxiv =
            let rand = System.Random()
            async {
                while true do
                    let iv = rand.Next(miniv, maxiv)
                    do! Async.Sleep iv
                    let a = rand.NextDouble() * range
                    s.Trigger (a)
            }
            |> Async.Start

        generate stream1 300. 1000 1250
        generate stream2 100. 1000 1050

        generateWithoutLabel stream3 100. 1000 1050
//
//        let months = [|"January"; "February"; "March"; "April"; "May"; "June";
//                       "July"; "August"; "September"; "October"; "November"; "December"|]
//        let data = Array.zip months [| for _ in 1 .. 12 -> 0.0 |] 
//        let chart = Chart.Bar data
//        Renderers.ChartJs.Render(chart, Size = Size(500, 350))
//            .AppendTo "entry" // where "main" is the id of some element in the DOM
//        
//        let rnd = System.Random()
//        async {
//            while true do
//                do! Async.Sleep 300
//                let i = rnd.Next(0,12)
//                let a = rnd.NextDouble() * 10.
//                chart.UpdateData(i, fun e -> e + a)
//        }
//        |> Async.Start

//        ChartJs.LineChartConfiguration(DatasetFill = true, BezierCurve = false)
//
//        Chart.Combine [
//            LiveChart.Line(source.Publish)
//                .WithFillColor(Color.Rgba(40, 40, 150, 0.2))
//            LiveChart.Line(averageByPoint)
//                .WithFillColor(Color.Rgba(40, 150, 40, 0.2))
//                .WithStrokeColor(Color.Name "blue")
//        ]
    
        let lc =
            Renderers.ChartJs.Render(
                LiveChart.Line(stream1.Publish)
                    .WithTitle("LiveChart example")
                    .WithFillColor(Color.Rgba(40, 40, 150, 0.2))
                    .WithPointColor(Color.Name "red")
                    .WithStrokeColor(Color.Name "green"),
                Window = 30
            )

        let combinedLive =
            Chart.Combine [
                LiveChart.Line(stream1.Publish)
                    .WithTitle("stream1")
                    .WithPointColor(Color.Name "yellow")
                    .WithStrokeColor(Color.Name "black")
                    .WithFill(false)
                LiveChart.Line(stream2.Publish)
                    .WithTitle("stream2")
                    .WithPointColor(Color.Name "red")
                    .WithStrokeColor(Color.Name "green")
                    .WithFill(false)
            ]

        let withoutLabel =
            Renderers.ChartJs.Render(
                LiveChart.Line(stream3.Publish)
                    .WithTitle("without label"),
                Window = 30
            )
            
        let data = [for x in 1.0 .. 9.0 -> (string x + "^2", x * x)]
        let chart =
            Chart.Line(data)
              .WithTitle("Square numbers")
              .WithFillColor(Color.Rgba(120, 120, 120, 0.2))
        let blueLine = chart.WithStrokeColor(Color.Name "blue")
        let redLine = chart.WithStrokeColor(Color.Name "red")

        div [] [
            chart |> Renderers.ChartJs.Render
            cr
            crs
            combinedBar
            renderedPie
            renderedDoughnut
            renderedPolar
            Renderers.ChartJs.Render(blueLine, Size = Size(300, 200))
            Renderers.ChartJs.Render(redLine, Size = Size(300, 200))
            hr [] []
            div [] [text "Live Charts"]
            lc
            Renderers.ChartJs.Render(combinedLive, Window = 30)
            withoutLabel
        ]
        
        |> Doc.RunById "entry"
