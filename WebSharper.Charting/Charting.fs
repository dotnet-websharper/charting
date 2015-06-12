namespace WebSharper.Charting

open WebSharper

[<JavaScript>]
module Charting =
    
    let LineChart mapping data =
        let (labels, dataset) =
            Seq.map mapping data
            |> Seq.toArray
            |> Array.unzip
        
        LineChart (labels, dataset)
