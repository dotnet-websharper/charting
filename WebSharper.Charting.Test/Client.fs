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
        
        let randomColor () =
            let tochar c =
                if c < 10 then string c
                else 
                    let b = (int 'a') + c - 10
                    string <| char b

            let to16bits (c : int) =
                let a = c / 16
                let b = c % 16
                (tochar a) + (tochar b)

            [ for _ in 1 .. 3 -> to16bits <| rand.Next 256 ]
            |> String.concat ""

        let area =
            [
                "pear", 32.
                "strawberry", 23.
                "blueberry", 5.
            ]
            |> List.map (fun (l, v) ->
                ChartJs.PolarAreaChartDataset(
                    Color = "#" + randomColor (),
                    Label = l,
                    Value = v
                ))
            |> BufferedStream.FromList
        
        Div [
            Chart.Bar s
            |> Chart.WithDimension (600, 300)
            |> Chart.WithWindow 5
        ] -< [
            Br []
        ] -< [
            Chart.Area area
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
