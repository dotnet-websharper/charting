namespace WebSharper.Charting

open System
open WebSharper
open IntelliFactory.Reactive

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
type Chart =
    static member Line(dataset) =
        Charts.LineChart(dataset)

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
