namespace WebSharper.Charting.Testt

open WebSharper
open WebSharper.JavaScript
open WebSharper.Charting
open IntelliFactory.Reactive

[<JavaScript>]
module Client =

    let Main =
        let stream = BufferedStream<float>.New 40

        let s = 
            Reactive.Select
            <| Reactive.Aggregate stream (0, 0.0) (fun (s, _) c -> (s + 1, c)) 
            <| fun (a, b) -> (string a, b)

        let chart = Chart.Line s

        let rand = System.Random()

        async {
            for i in 1 .. 20 do
                do! Async.Sleep 1200
                stream.Trigger <| (rand.NextDouble() * 300.)
        }
        |> Async.Start

        chart
        |> Chart.WithDimension (600, 300)
        |> fun el -> el.AppendTo "entry"
