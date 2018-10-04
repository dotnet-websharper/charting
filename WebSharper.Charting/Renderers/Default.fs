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
open WebSharper.JavaScript

[<JavaScript>]
module Renderers =
    
    type LineChart () =
        let canvas = Canvas []

        let mutable chart = null
        let mutable pc = 0
        let mutable window = None

        interface IRenderer<string * float> with
            member this.Body = canvas    

            member this.Render cs =
                canvas.Body?width  <- cs.Width
                canvas.Body?height <- cs.Height

                window <- cs.Window

                let data =
                    ChartJs.LineChartData(
                        Labels   = Array.empty,
                        Datasets =
                            [|
                                ChartJs.LineChartDataset(Data = Array.empty)
                            |]
                    )

                let options =
                    ChartJs.LineChartConfiguration(
                        BezierCurve = false,
                        DatasetFill = false
                    )

                let context =
                    (As<CanvasElement> canvas.Body).GetContext("2d")

                chart <- ChartJs.Chart(context).Line(data, options)

            member this.AddData((label, data)) =
                if window.IsSome then
                    if pc >= window.Value then chart.RemoveData()

                pc <- pc + 1

                chart.AddData([| data |], label)

    type PieChart () =
        let canvas = Canvas []

        let mutable chart = null

        interface IRenderer<int * ChartJs.PolarAreaChartDataset> with
            member this.Body = canvas    

            member this.Render cs =
                canvas.Body?width  <- cs.Width
                canvas.Body?height <- cs.Height

                let data : ChartJs.PieChartDataset [] = [||]
                let options =
                    ChartJs.PieChartConfiguration()

                let context =
                    (As<CanvasElement> canvas.Body).GetContext("2d")

                chart <- ChartJs.Chart(context).Pie(data, options)

            member this.AddData((idx, data)) =
                chart.AddData(data, idx)

    type AreaChart () =
        let canvas = Canvas []

        let mutable chart = null

        interface IRenderer<int * ChartJs.PolarAreaChartDataset> with
            member this.Body = canvas

            member this.Render cs =
                canvas.Body?width  <- cs.Width
                canvas.Body?height <- cs.Height

                let data : ChartJs.PolarAreaChartDataset [] = [||]
                let options =
                    ChartJs.PolarAreaChartConfiguration()

                let context =
                    (As<CanvasElement> canvas.Body).GetContext("2d")

                chart <- ChartJs.Chart(context).PolarArea(data, options)

            member this.AddData((idx, data)) =
                chart.AddData(data, idx)

    type BarChart () =
        let canvas = Canvas []

        let mutable chart = null
        let mutable pc = 0
        let mutable window = None

        interface IRenderer<string * float> with
            member this.Body = canvas    

            member this.Render cs =
                canvas.Body?width  <- cs.Width
                canvas.Body?height <- cs.Height

                let data =
                    ChartJs.BarChartData(
                        Labels = Array.empty,
                        Datasets = [|ChartJs.BarChartDataset(Data = Array.empty)|]
                    )

                let options =
                    ChartJs.BarChartConfiguration()

                let context =
                    (As<CanvasElement> canvas.Body).GetContext("2d")

                chart <- ChartJs.Chart(context).Bar(data, options)

            member this.AddData((label, data)) =
                if window.IsSome then
                    if pc >= window.Value then chart.RemoveData()

                pc <- pc + 1
                chart.AddData([| data |], label)
