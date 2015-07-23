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
        let renderer = 
            Renderers.ChartJs.LineChartRenderer(Size(500, 200))

        Chart.Line([ for x in 1.0 .. 10.0 -> (string x, x ** 2.0) ])
            .WithStrokeColor(Color.Name "blue")
        |> renderer.Render
        |> insert

        // live chart

        let liveRenderer =
            Renderers.ChartJs.Live.LineChartRenderer(Size(500, 200), 10)

        let stream = BufferedStream<float>(40)
        
        stream
        |> LiveChart.Line
        |> liveRenderer.Render
        |> insert

        Reactive.Select
        <| Reactive.Aggregate stream (0, 0.0) (fun (s, e) c -> 
            let fs = float s
            (s + 1, (e * fs + c) / (fs + 1.)))
        <| fun (a, b) -> (string a, b)
        |> LiveChart.Line
        |> liveRenderer.Render
        |> insert

        let rand = System.Random()
        async {
            while true do
                let iv = rand.Next(600, 1500)
                do! Async.Sleep iv
                stream.Trigger <| (rand.NextDouble() * 300.)
        }
        |> Async.Start


        

