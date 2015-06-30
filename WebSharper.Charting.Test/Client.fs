namespace WebSharper.Charting.Test

open WebSharper
open WebSharper.JavaScript
open WebSharper.Html.Client
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

        let avg =
            Reactive.Select
            <| Reactive.Aggregate stream (0, 0.0) (fun (s, e) c -> 
                let fs = float s
                (s + 1, (e * fs + c) / (fs + 1.)))
            <| fun (a, b) -> (string a, b)

        let rand = System.Random()

        async {
            for i in 1 .. 20 do
                let iv = rand.Next(300, 2500)
                do! Async.Sleep iv
                stream.Trigger <| (rand.NextDouble() * 300.)
        }
        |> Async.Start
        
        
        Div [
            Chart.Line s
            |> Chart.WithDimension (600, 300)
            |> Chart.WithWindow 5
        ] -< [
            Br []
        ]
        |>! OnAfterRender (fun el ->
            async {
                do! Async.Sleep 3000
                let ch = 
                    Chart.Line avg
                    |> Chart.WithDimension (600, 300)
                el.Append ch
            }
            |> Async.Start
        )
        |> fun el -> el.AppendTo "entry"
