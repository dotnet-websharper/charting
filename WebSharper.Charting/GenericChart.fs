// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2018 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}
namespace WebSharper.Charting

open WebSharper
open WebSharper.Html.Client

[<JavaScript>]
type GenericChart<'T> private (state) =
    inherit Pagelet()

    member val Stream = state.Dataset

    override this.Body = state.Renderer.Body.Body

    override this.Render () =
        state.Renderer.Render state
        // TODO memory leak
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
                Window = None
            }

        GenericChart(state)

    member val State = state
