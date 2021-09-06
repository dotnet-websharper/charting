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

[<JavaScript>]
type Chart =

    static member Line dataset =
        GenericChart(Renderers.LineChart(), BufferedStream.FromList dataset)
    
    static member Line (dataset : System.IObservable<float>) =
        let s = 
            Reactive.Select
            <| Reactive.Aggregate dataset (0, 0.0) (fun (s, _) c -> (s + 1, c))
            <| fun (a, b) -> (string a, b)
        GenericChart(Renderers.LineChart(), s)

    static member Line dataset =
        GenericChart(Renderers.LineChart(), dataset)

    static member Bar (dataset : System.IObservable<float>) =
        let s = 
            Reactive.Select
            <| Reactive.Aggregate dataset (0, 0.0) (fun (s, _) c -> (s + 1, c))
            <| fun (a, b) -> (string a, b)
        GenericChart(Renderers.BarChart(), s)

    static member Bar dataset =
        GenericChart(Renderers.BarChart(), dataset)

    static member Pie (dataset : System.IObservable<ChartJs.PolarAreaChartDataset>) =
        let s = 
            Reactive.Aggregate dataset (0, null) (fun (s, _) c -> (s + 1, c))
        GenericChart(Renderers.PieChart(), s)

    static member Pie dataset =
        GenericChart(Renderers.PieChart(), dataset)

    static member Area (dataset : System.IObservable<ChartJs.PolarAreaChartDataset>) =
        let s = 
            Reactive.Aggregate dataset (0, null) (fun (s, _) c -> (s + 1, c))
        GenericChart(Renderers.AreaChart(), s)

    static member Area dataset =
        GenericChart(Renderers.AreaChart(), dataset)

    static member WithWindow window (chart : GenericChart<_>) =
        { chart.State with
            Window = Some window }
        |> GenericChart.FromState

    static member WithRenderer renderer (chart : GenericChart<_>) =
        { chart.State with
            Renderer = renderer }
        |> GenericChart.FromState
    
    static member WithDimension (width, height) (chart : GenericChart<_>) =
        { chart.State with
            Width  = width
            Height = height }
        |> GenericChart.FromState
