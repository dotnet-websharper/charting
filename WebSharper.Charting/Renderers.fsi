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

module Renderers = begin
    
    /// <summary>Functions for rendering charts with Chart.js.</summary>
    type ChartJs =
        class
            /// <summary>Renders a LineChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.LineChart * ?Size:Pervasives.Size * ?Config:ChartJs.Options * ?Window:int -> WebSharper.UI.Doc

//            /// <summary>Renders a BarChart instance.</summary>
//            /// <parameter name="chart">The chart to be rendered.</parameter>
//            /// <parameter name="Size">The Size of the canvas.</parameter>
//            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
//            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.BarChart * ?Size:Pervasives.Size * ?Config:ChartJs.Options * ?Window:int -> WebSharper.UI.Doc
//            
//            /// <summary>Renders a RadarChart instance.</summary>
//            /// <parameter name="chart">The chart to be rendered.</parameter>
//            /// <parameter name="Size">The Size of the canvas.</parameter>
//            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
//            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.RadarChart * ?Size:Pervasives.Size * ?Config:ChartJs.RadarChartOptions * ?Window:int -> WebSharper.UI.Doc
           
            /// <summary>Renders a PieChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.PieChart * ?Size:Pervasives.Size * ?Config:ChartJs.PieDoughnutChartOptions * ?Window:int -> WebSharper.UI.Doc
            
            /// <summary>Renders a DoughnutChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.DoughnutChart * ?Size:Pervasives.Size * ?Config:ChartJs.PieDoughnutChartOptions * ?Window:int -> WebSharper.UI.Doc
            
            /// <summary>Renders a PolarAreaChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.PolarAreaChart * ?Size:Pervasives.Size * ?Config:ChartJs.PolarAreaChartOptions * ?Window:int -> WebSharper.UI.Doc
//            
            /// <summary>Renders a combined LineChart.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.CompositeChart<Charts.LineChart> * ?Size:Pervasives.Size * ?Config:ChartJs.Options * ?Window:int -> WebSharper.UI.Doc

            /// <summary>Renders a combined BarChart.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.CompositeChart<Charts.BarChart> * ?Size:Pervasives.Size * ?Config:ChartJs.Options * ?Window:int -> WebSharper.UI.Doc

            /// <summary>Renders a combined RadarChart.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.CompositeChart<Charts.RadarChart> * ?Size:Pervasives.Size * ?Config:ChartJs.RadarChartOptions * ?Window:int -> WebSharper.UI.Doc
        end

    type Plotly =
        class
            /// <summary>Renders a LineChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.LineChart * ?Size:Pervasives.Size * ?Config:Plotly.Options * ?Window:int -> WebSharper.UI.Doc

//            /// <summary>Renders a BarChart instance.</summary>
//            /// <parameter name="chart">The chart to be rendered.</parameter>
//            /// <parameter name="Size">The Size of the canvas.</parameter>
//            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
//            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.BarChart * ?Size:Pervasives.Size * ?Config:ChartJs.Options * ?Window:int -> WebSharper.UI.Doc
//            
//            /// <summary>Renders a RadarChart instance.</summary>
//            /// <parameter name="chart">The chart to be rendered.</parameter>
//            /// <parameter name="Size">The Size of the canvas.</parameter>
//            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
//            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.RadarChart * ?Size:Pervasives.Size * ?Config:ChartJs.RadarChartOptions * ?Window:int -> WebSharper.UI.Doc
           
            /// <summary>Renders a PieChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.PieChart * ?Size:Pervasives.Size * ?Config:ChartJs.PieDoughnutChartOptions * ?Window:int -> WebSharper.UI.Doc
            
            /// <summary>Renders a DoughnutChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.DoughnutChart * ?Size:Pervasives.Size * ?Config:ChartJs.PieDoughnutChartOptions * ?Window:int -> WebSharper.UI.Doc
            
            /// <summary>Renders a PolarAreaChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.PolarAreaChart * ?Size:Pervasives.Size * ?Config:ChartJs.PolarAreaChartOptions * ?Window:int -> WebSharper.UI.Doc
//            
            /// <summary>Renders a combined LineChart.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.CompositeChart<Charts.LineChart> * ?Size:Pervasives.Size * ?Config:ChartJs.Options * ?Window:int -> WebSharper.UI.Doc

            /// <summary>Renders a combined BarChart.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.CompositeChart<Charts.BarChart> * ?Size:Pervasives.Size * ?Config:ChartJs.Options * ?Window:int -> WebSharper.UI.Doc

            /// <summary>Renders a combined RadarChart.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.CompositeChart<Charts.RadarChart> * ?Size:Pervasives.Size * ?Config:ChartJs.RadarChartOptions * ?Window:int -> WebSharper.UI.Doc
        end
end
