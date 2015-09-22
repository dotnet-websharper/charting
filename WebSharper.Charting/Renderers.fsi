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
            static member Render : chart:Charts.LineChart * ?Size:Pervasives.Size * ?Config:ChartJs.LineChartConfiguration * ?Window:int -> WebSharper.UI.Next.Elt

            /// <summary>Renders a BarChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.BarChart * ?Size:Pervasives.Size * ?Config:ChartJs.BarChartConfiguration * ?Window:int -> WebSharper.UI.Next.Elt
            
            /// <summary>Renders a RadarChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.RadarChart * ?Size:Pervasives.Size * ?Config:ChartJs.RadarChartConfiguration * ?Window:int -> WebSharper.UI.Next.Elt
            
            /// <summary>Renders a PieChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.PieChart * ?Size:Pervasives.Size * ?Config:ChartJs.PieChartConfiguration * ?Window:int -> WebSharper.UI.Next.Elt
            
            /// <summary>Renders a DoughnutChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.DoughnutChart * ?Size:Pervasives.Size * ?Config:ChartJs.DoughnutChartConfiguration * ?Window:int -> WebSharper.UI.Next.Elt
            
            /// <summary>Renders a PolarAreaChart instance.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.PolarAreaChart * ?Size:Pervasives.Size * ?Config:ChartJs.PolarAreaChartConfiguration * ?Window:int -> WebSharper.UI.Next.Elt
            
            /// <summary>Renders a combined LineChart.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.CompositeChart<Charts.LineChart> * ?Size:Pervasives.Size * ?Config:ChartJs.LineChartConfiguration * ?Window:int -> WebSharper.UI.Next.Elt

            /// <summary>Renders a combined BarChart.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.CompositeChart<Charts.BarChart> * ?Size:Pervasives.Size * ?Config:ChartJs.BarChartConfiguration * ?Window:int -> WebSharper.UI.Next.Elt

            /// <summary>Renders a combined RadarChart.</summary>
            /// <parameter name="chart">The chart to be rendered.</parameter>
            /// <parameter name="Size">The Size of the canvas.</parameter>
            /// <parameter name="Config">ChartJs configuration for rendering the chart.</parameter>
            /// <parameter name="Window">How many data points should be rendered max.</parameter>
            static member Render : chart:Charts.CompositeChart<Charts.RadarChart> * ?Size:Pervasives.Size * ?Config:ChartJs.RadarChartConfiguration * ?Window:int -> WebSharper.UI.Next.Elt
        end
end
