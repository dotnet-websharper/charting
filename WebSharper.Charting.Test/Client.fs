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

//        let data = [ for x in 1.0 .. 10.0 -> (string x, x ** 2.0) ]
//
//        Chart.PolarArea(data)
//        |> Renderers.ChartJs.Render
//        |> insert
//
//        Chart.Pie(data)
//        |> Renderers.ChartJs.Render
//        |> insert
//
//        Chart.Doughnut(data)
//        |> Renderers.ChartJs.Render
//        |> insert
//
//        Chart.Radar(List.zip ["apples"; "oranges"; "strawberries"; "appricots"] [12.; 32.; 6.; 2.])
//            .WithFillColor(Color.Name "red")
//            .WithPointColor(Color.Name "black")
//        |> fun c -> Renderers.ChartJs.Render(c, Size = Size(500, 500))
//        |> insert

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
        // live charts

        let stream1 = BufferedStream<float>(40)
        let stream2 = BufferedStream<float>(40)

        [
            LiveChart.Radar stream1
            LiveChart.Radar(stream2)
                .WithStrokeColor(Color.Name "red")
                .WithFillColor(Color.Rgba(100, 160, 100, 0.1))
        ]
        |> Chart.Combine
        |> fun ch -> Renderers.ChartJs.Render(ch, Window = 10)
        |> insert

//
//        let ds = Reactive.Select stream (fun e -> (string e, e))
//        
//        LiveChart.Pie(ds)
//        |> fun c -> Renderers.ChartJs.Render(c, Window = 10)
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
        let generate (s : Event<float>) range miniv maxiv =
            let rand = System.Random()
            async {
                while true do
                    let iv = rand.Next(miniv, maxiv)
                    do! Async.Sleep iv
                    let a = rand.NextDouble() * range
                    s.Trigger a
            }
            |> Async.Start

        generate stream1.Event 300. 200 250
        generate stream2.Event 100. 1000 1050


        

