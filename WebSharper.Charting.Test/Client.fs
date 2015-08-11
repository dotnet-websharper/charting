namespace WebSharper.Charting.Test

open WebSharper
open WebSharper.JavaScript
open WebSharper.Html.Client
open WebSharper.Charting
open IntelliFactory.Reactive

[<Interface>]
type IChart<'Self when 'Self :> IChart<'Self>> =
    abstract member WithTitle : string -> 'Self

type EmptyChart(title : string) =
    interface IChart<EmptyChart> with
        override x.WithTitle t = EmptyChart(t)

[<JavaScript>]
module Client =

    let insert (el : Element) =
        el.AppendTo "entry"

    let Main =
        let data = [ for x in 1.0 .. 9.0 -> (string x, x ** 2.0) ]

        let chart = Chart.PolarArea(data)

        chart
        |> Renderers.ChartJs.Render
        |> insert

        let ch1 = 
            Chart.Line([ for x in 1.0 .. 11.0 -> (string x, x) ])
                .WithFillColor(Color.Rgba(32, 64, 128, 1.))

        let ch2 =
            Chart.Line([ for x in 1.0 .. 10.0 -> (string x, x * x) ])
                .WithFillColor(Color.Rgba(128, 64, 32, 0.1))

        [

        ]
        |> Chart.Combine
        |> fun e -> 
            let c = ChartJs.LineChartConfiguration(DatasetFill = true)
            Renderers.ChartJs.Render(e, Config = c)
        |> insert

        let stream1 = Event<float>()
        let stream2 = Event<float>()


        let b1 = LiveChart.Radar stream1.Publish
        let b2 =
            LiveChart.Radar(stream2.Publish)
                .WithStrokeColor(Color.Name "red")
                .WithFillColor(Color.Rgba(100, 160, 100, 0.1))

        [
            b1
            b2
        ]
        |> Chart.Combine
        |> fun ch -> Renderers.ChartJs.Render(ch, Window = 10)
        |> insert

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

        generate stream1 300. 1000 1250
        generate stream2 100. 1000 1050

        let months = [|"January"; "February"; "March"; "April"; "May"; "June";
                       "July"; "August"; "September"; "October"; "November"; "December"|]
        let data = Array.zip months [| for _ in 1 .. 12 -> 0.0 |] 
        let chart = Chart.Bar data
        Renderers.ChartJs.Render(chart, Size = Size(500, 350))
            .AppendTo "entry" // where "main" is the id of some element in the DOM
        
        let rnd = System.Random()
        async {
            while true do
                do! Async.Sleep 300
                let i = rnd.Next(0,12)
                let a = rnd.NextDouble() * 10.
                chart.UpdateData(i, fun e -> e + a)
        }
        |> Async.Start
