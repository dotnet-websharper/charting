namespace WebSharper.Charting.Test

open WebSharper
open WebSharper.JavaScript
open WebSharper.Html.Client
open WebSharper.Charting
open IntelliFactory.Reactive

[<JavaScript>]
module Client =

    let insert (el : Element) =
        el.AppendTo "entry"

    let Main =

        let data = [ for x in 1.0 .. 10.0 -> (string x, x ** 2.0) ]

        Chart.PolarArea(data)
            .WithStrokeColor(Color.Name "blue")
        |> Renderers.ChartJs.Render
        |> insert

        Chart.Pie(data)
        |> Renderers.ChartJs.Render
        |> insert

        Chart.Doughnut(data)
            .WithStrokeColor(Color.Name "blue")
        |> Renderers.ChartJs.Render
        |> insert

        Chart.Radar(List.zip ["apples"; "oranges"; "strawberries"; "appricots"] [12.; 32.; 6.; 2.])
            .WithFillColor(Color.Name "yellow")
        |> fun c -> Renderers.ChartJs.Render(c, Size = Size(500, 500))
        |> insert

//        [
//            Chart.Line([ for x in 1.0 .. 9.0 -> (string x, x) ])
//                .WithFillColor(Color.Rgba(32, 64, 128, 1.))
//
//            Chart.Line([ for x in 1.0 .. 10.0 -> (string x, x * x) ])
//                .WithFillColor(Color.Rgba(128, 64, 32, 0.1))
//        ]
//        |> Chart.Combine
//        |> fun e -> 
//            let c = ChartJs.LineChartConfiguration(DatasetFill = true)
//            Renderers.ChartJs.Render(e, Config = c)
//        |> insert
//
//        // live charts
//
//        let stream = BufferedStream<float>(40)
//        
//        LiveChart.Line(stream)
//            .WithFillColor(Color.Name "yellow")
//        |> Renderers.ChartJs.Render
//        |> insert
//
//        stream
//        |> LiveChart.Line
//        |> fun chrt -> Renderers.ChartJs.Render(chrt, Window = 10)
//        |> insert
//
//        Reactive.Select
//        <| Reactive.Aggregate stream (0, 0.0) (fun (s, e) c -> 
//            let fs = float s
//            (s + 1, (e * fs + c) / (fs + 1.)))
//        <| fun (a, b) -> (string a, b)
//        |> LiveChart.Line
//        |> Renderers.ChartJs.Render
//        |> insert
//
//        let rand = System.Random()
//        async {
//            while true do
//                let iv = rand.Next(600, 1500)
//                do! Async.Sleep iv
//                stream.Trigger <| (rand.NextDouble() * 300.)
//        }
//        |> Async.Start


        

