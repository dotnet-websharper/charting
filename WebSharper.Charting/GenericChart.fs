namespace WebSharper.Charting

open WebSharper
open WebSharper.Html.Client

[<JavaScript>]
type GenericChart<'T> private (state) =
    inherit Pagelet()
    
    override this.Body = state.Renderer.Body.Body
    
    override this.Render () =
        state.Renderer.Render   state
        state.Dataset.Subscribe this

    internal new (renderer, dataset) =
        GenericChart({ Renderer = renderer; Dataset = dataset; Type = Line })
    
    interface IObserver<'T> with
        member this.OnChange stream =
            state.Renderer.AddData stream.LastValue
            