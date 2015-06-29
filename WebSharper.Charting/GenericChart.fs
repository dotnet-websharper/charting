namespace WebSharper.Charting

open WebSharper
open WebSharper.Html.Client

[<JavaScript>]
type GenericChart<'T> private (state) =
    inherit Pagelet()

    override this.Body = state.Renderer.Body.Body
    
    override this.Render () =
        state.Renderer.Render state
        state.Dataset.Add <| fun (t :'T) -> state.Renderer.AddData t
    
    static member internal FromState state =
        GenericChart(state)

    internal new (renderer, dataset) =
        let state =
            {
                Renderer = renderer
                Dataset  = dataset
                Type = Line
                Width  = 400
                Height = 300
            }

        GenericChart(state)

    member val State = state
