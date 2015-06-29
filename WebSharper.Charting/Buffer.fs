namespace WebSharper.Charting

open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module internal Array =
    
    [<Inline "$1.push($0)">]
    let push (_ : 'T) (_ : 'T array) = ()

    [<Inline "$0.shift()">]
    let shift (_ : 'T array) = ()

[<JavaScript>]
type private Buffer<'T> (capacity) =
    let backing : 'T array = Array.empty

    new () = Buffer(500)

    member this.Push (value : 'T) =
        backing
        |> Array.push value

        if backing.Length > capacity then
            Array.shift backing
            true
        else
            false
    
    member this.State = backing
