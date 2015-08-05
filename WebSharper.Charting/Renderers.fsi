namespace WebSharper.Charting

open WebSharper

/// Available rendering methods.
module Renderers = begin
    
    /// Functions for rendering the internal chart-type with Chart.js.
    type ChartJs =
        class
            /// Renders a LineChart instance with the given options using Chart.js.
            /// For the configuration options, please refer to the Chart.js website.
            static member Render : chart:Charts.LineChart * ?Size:Pervasives.Size * ?Config:ChartJs.LineChartConfiguration * ?Window:int -> Html.Client.Element

            /// Renders a BarChart instance with the given options using Chart.js.
            /// For the configuration options, please refer to the Chart.js website.
            static member Render : chart:Charts.BarChart * ?Size:Pervasives.Size * ?Config:ChartJs.BarChartConfiguration * ?Window:int -> Html.Client.Element
            
            /// Renders a RadarChart instance with the given options using Chart.js.
            /// For the configuration options, please refer to the Chart.js website.
            static member Render : chart:Charts.RadarChart * ?Size:Pervasives.Size * ?Config:ChartJs.RadarChartConfiguration * ?Window:int -> Html.Client.Element
            
            /// Renders a PieChart instance with the given options using Chart.js.
            /// For the configuration options, please refer to the Chart.js website.
            static member Render : chart:Charts.PieChart * ?Size:Pervasives.Size * ?Config:ChartJs.PieChartConfiguration * ?Window:int -> Html.Client.Element
            
            /// Renders a DoughnutChart instance with the given options using Chart.js.
            /// For the configuration options, please refer to the Chart.js website.
            static member Render : chart:Charts.DoughnutChart * ?Size:Pervasives.Size * ?Config:ChartJs.DoughnutChartConfiguration * ?Window:int -> Html.Client.Element
            
            /// Renders a PolarAreaChart instance with the given options using Chart.js.
            /// For the configuration options, please refer to the Chart.js website.
            static member Render : chart:Charts.PolarAreaChart * ?Size:Pervasives.Size * ?Config:ChartJs.PolarAreaChartConfiguration * ?Window:int -> Html.Client.Element
            
            /// Renders a combined line-chart with the given options using Chart.js.
            /// For the configuration options, please refer to the Chart.js website.
            static member Render : chart:Charts.CompositeChart<Charts.LineChart> * ?Size:Pervasives.Size * ?Config:ChartJs.LineChartConfiguration * ?Window:int -> Html.Client.Element
        end
end
