namespace WebSharper.Charting

module Charts = begin
    
    /// <summary>A segment in a polar-area chart (pie chart, doughnut chart).</summary>
    type PolarData =
        {
            Value     : float
            Color     : Pervasives.Color
            Highlight : Pervasives.Color
            Label     : string
        }
    
    /// <summary>The type of data to show in a chart.</summary>
    type DataType<'T> =
        /// <summary>Static data to which you cannot add points later.</summary>
        | Static of seq<'T>
        /// <summary>Live data which can be updated with new elements in real-time.</summary>
        | Live of System.IObservable<'T>

    module DataType = begin
        /// <summary>Builds a new colelction whose elements are the results of appying the given
        /// functions to each element of the collection. Preserves the type of the source.</summary>
        val Map : fn:('a -> 'b) -> dt:DataType<'a> -> DataType<'b>
    end


    /// <summary>Configuration of a generic chart.</summary>
    type ChartConfig =
        {
            Title: string
        }

    /// <summary>Configuration of a series chart.</summary>
    type SeriesChartConfig =
        {
            XAxis: string
            YAxis: string
            FillColor: Pervasives.Color
            StrokeColor: Pervasives.Color
        }

    /// <summary>Color configuration of a chart.</summary>
    type ColorConfig =
        {
            PointColor: Pervasives.Color
            PointHighlightFill: Pervasives.Color
            PointHighlightStroke: Pervasives.Color
            PointStrokeColor: Pervasives.Color
        }

    /// <summary>Base type of all charts.</summary>
    /// <typeparam name="Self">The own type of the implementing class.</typeparam>
    type IChart<'Self when 'Self :> IChart<'Self>> =
        interface
            abstract member WithTitle : string -> 'Self
            abstract member Config : ChartConfig
        end

    /// <summary>Chart that can be colored with ColorConfig.</summary>
    type IColorChart<'Self when 'Self :> IColorChart<'Self>> =
        interface
            inherit IChart<'Self>

            abstract member WithPointColor : Pervasives.Color -> 'Self
            abstract member WithPointHighlightFill : Pervasives.Color -> 'Self
            abstract member WithPointHighlightStroke : Pervasives.Color -> 'Self
            abstract member WithPointStrokeColor : Pervasives.Color -> 'Self
            abstract member ColorConfig : ColorConfig
        end

    type ISeriesChart<'Self when 'Self :> ISeriesChart<'Self>> =
        interface
            inherit IChart<'Self>

            abstract member WithFillColor : Pervasives.Color -> 'Self
            abstract member WithStrokeColor : Pervasives.Color -> 'Self
            abstract member WithXAxis : string -> 'Self
            abstract member WithYAxis : string -> 'Self
            abstract member SeriesConfig : SeriesChartConfig
        end

    /// <summary>Chart containing mutable data.</summary>
    type IMutableChart<'T, 'U> =
        /// <summary>Updates chart data at the given position.</summary>
        /// <param name="props">Extra properties needed for updating (such as index).</param>
        /// <param name="data">The data to insert at the given position.</param>
        abstract member UpdateData : props : 'U * data :'T -> unit
        /// <summary>Adds a listener to the chart that gets called on data updates.</summary>
        abstract member OnUpdate : ('U * 'T -> unit) -> unit

    type LineChart =
        class
            interface IColorChart<LineChart>
            interface ISeriesChart<LineChart>
            interface IChart<LineChart>
            interface IMutableChart<float, int>
        
            /// <summary>Updates chart data at the given position.</summary>
            /// <param name="props">Extra properties needed for updating (such as index).</param>
            /// <param name="data">The data to insert at the given position.</param>
            member UpdateData : props : int * data : float -> unit
            member WithFillColor : color:Pervasives.Color -> LineChart
            member WithPointColor : color:Pervasives.Color -> LineChart
            member WithPointHighlightFill : color:Pervasives.Color -> LineChart
            member WithPointHighlightStroke : color:Pervasives.Color -> LineChart
            member WithPointStrokeColor : color:Pervasives.Color -> LineChart
            member WithStrokeColor : color:Pervasives.Color -> LineChart
            member WithTitle : title:string -> LineChart
            member WithXAxis : xAxis:string -> LineChart
            member WithYAxis : yAxis:string -> LineChart
            member ColorConfig : ColorConfig
            member Config : ChartConfig
            member DataSet : DataType<string * float>
            member SeriesConfig : SeriesChartConfig
        end

    type BarChart =
        class
            interface ISeriesChart<BarChart>
            interface IMutableChart<float, int>

            /// <summary>Updates chart data at the given position.</summary>
            /// <param name="props">Extra properties needed for updating (such as index).</param>
            /// <param name="data">The data to insert at the given position.</param>
            member UpdateData : props : int * data : float -> unit
            member WithFillColor : color:Pervasives.Color -> BarChart
            member WithStrokeColor : color:Pervasives.Color -> BarChart
            member WithTitle : title:string -> BarChart
            member WithXAxis : xAxis:string -> BarChart
            member WithYAxis : yAxis:string -> BarChart
            member Config : ChartConfig
            member DataSet : DataType<string * float>
            member SeriesConfig : SeriesChartConfig
        end

    type RadarChart =
        class
            interface IColorChart<RadarChart>
            interface ISeriesChart<RadarChart>
            interface IChart<RadarChart>
            interface IMutableChart<float, int>
        
            /// <summary>Updates chart data at the given position.</summary>
            /// <param name="props">Extra properties needed for updating (such as index).</param>
            /// <param name="data">The data to insert at the given position.</param>
            member UpdateData : props : int * data : float -> unit
            member WithFillColor : color:Pervasives.Color -> RadarChart
            member WithPointColor : color:Pervasives.Color -> RadarChart
            member WithPointHighlightFill : color:Pervasives.Color -> RadarChart
            member WithPointHighlightStroke : color:Pervasives.Color -> RadarChart
            member WithPointStrokeColor : color:Pervasives.Color -> RadarChart
            member WithStrokeColor : color:Pervasives.Color -> RadarChart
            member WithTitle : title:string -> RadarChart
            member WithXAxis : xAxis:string -> RadarChart
            member WithYAxis : yAxis:string -> RadarChart
            member ColorConfig : ColorConfig
            member Config : ChartConfig
            member DataSet : DataType<string * float>
            member SeriesConfig : SeriesChartConfig
        end

    type IPolarAreaChart<'Self when 'Self :> IPolarAreaChart<'Self>> =
        interface
            inherit IChart<'Self>
            inherit IMutableChart<float, int>

            abstract member DataSet : DataType<PolarData>
        end

    type PolarAreaChart =
        class
           interface IPolarAreaChart<PolarAreaChart>
           interface IMutableChart<float, int>
        
            /// <summary>Updates chart data at the given position.</summary>
            /// <param name="props">Extra properties needed for updating (such as index).</param>
            /// <param name="data">The data to insert at the given position.</param>
            member UpdateData : props : int * data : float -> unit
            member WithTitle : title:string -> PolarAreaChart
            member Config : ChartConfig
            member DataSet : DataType<PolarData>
        end

    type PieChart =
        class
            interface IPolarAreaChart<PieChart>
            interface IMutableChart<float, int>

            /// <summary>Updates chart data at the given position.</summary>
            /// <param name="props">Extra properties needed for updating (such as index).</param>
            /// <param name="data">The data to insert at the given position.</param>
            member UpdateData : props : int * data : float -> unit
            member WithTitle : title:string -> PieChart
            member Config : ChartConfig
            member DataSet : DataType<PolarData>
        end

    type DoughnutChart =
        class
            interface IPolarAreaChart<DoughnutChart>
            interface IMutableChart<float, int>

            /// <summary>Updates chart data at the given position.</summary>
            /// <param name="props">Extra properties needed for updating (such as index).</param>
            /// <param name="data">The data to insert at the given position.</param>
            member UpdateData : props : int * data : float -> unit
            member WithTitle : title:string -> DoughnutChart
            member Config : ChartConfig
            member DataSet : DataType<PolarData>
        end

    type CompositeChart<'T when 'T :> ISeriesChart<'T>> =
        class
            member Charts : seq<'T>
        end
end

type Chart =
    class
        /// <summary>Creates a new bar chart from label and value pairs.</summary>
        static member Bar : dataset:seq<string * float> -> Charts.BarChart

        /// <summary>Combines the given series charts into one. You can combine live charts with static ones.</summary>
        static member Combine : charts:seq<'a> -> Charts.CompositeChart<'a> when 'a :> Charts.ISeriesChart<'a>

        /// <summary>Creates a new doughnut chart from a sequence of polar data.</summary>
        static member Doughnut : dataset:seq<Charts.PolarData> -> Charts.DoughnutChart

        /// <summary>Creates a new doughnut chart from label and value pairs.</summary>
        static member Doughnut : dataset:seq<string * float> -> Charts.DoughnutChart

        /// <summary>Creates a new line chart from label and value pairs.</summary>
        static member Line : dataset:seq<string * float> -> Charts.LineChart

        /// <summary>Creates a new pie chart from a sequence of polar data.</summary>
        static member Pie : dataset:seq<Charts.PolarData> -> Charts.PieChart

        /// <summary>Creates a new pie chart from label and value pairs.</summary>
        static member Pie : dataset:seq<string * float> -> Charts.PieChart

        /// <summary>Creates a new polar-area chart from a sequence of polar data.</summary>
        static member PolarArea : dataset:seq<Charts.PolarData> -> Charts.PolarAreaChart

        /// <summary>Creates a new polar-area chart from label and value pairs.</summary>
        static member PolarArea : dataset:seq<string * float> -> Charts.PolarAreaChart
          
        /// <summary>Creates a new radar chart from label and value pairs.</summary>
        static member Radar : dataset:seq<string * float> -> Charts.RadarChart
    end

type LiveChart =
    class
        /// <summary>Creates a new bar chart from stream of label and value pairs.</summary>
        static member Bar : dataset:System.IObservable<string * float> -> Charts.BarChart
            
        /// <summary>Creates a new bar chart from a stream of values.</summary>
        /// <remakrs>The labels will be an auto-generated series from the index of the element.</remakrs>
        static member Bar : dataset:System.IObservable<float> ->Charts.BarChart

        /// <summary>Creates a new doughnut chart from a stream of polar data.</summary>
        static member Doughnut : dataset:System.IObservable<Charts.PolarData> -> Charts.DoughnutChart
            
        /// <summary>Creates a new doughnut chart from a stream of label and value pairs.</summary>
        static member Doughnut : dataset:System.IObservable<string * float> -> Charts.DoughnutChart
            
        /// <summary>Creates a new line chart from a stream of label and value pairs.</summary>
        static member Line : dataset:System.IObservable<string * float> -> Charts.LineChart

        /// <summary>Creates a new line chart from a stream of values.</summary>
        /// <remakrs>The labels will be an auto-generated series from the index of the element.</remakrs>
        static member Line : dataset:System.IObservable<float> -> Charts.LineChart

        /// <summary>Creates a new pie chart from a stream of polar data.</summary>
        static member Pie : dataset:System.IObservable<Charts.PolarData> -> Charts.PieChart
            
        /// <summary>Creates a new pie chart from a stream of label and value pairs.</summary>
        static member Pie : dataset:System.IObservable<string * float> -> Charts.PieChart
            
        /// <summary>Creates a new polar-area chart from a stream of polar data.</summary>
        static member PolarArea : dataset:System.IObservable<Charts.PolarData> -> Charts.PolarAreaChart
            
        /// <summary>Creates a new polar-area chart from a stream of label and value pairs.</summary>
        static member PolarArea : dataset:System.IObservable<string * float> -> Charts.PolarAreaChart
            
        /// <summary>Creates a new radar chart from a stream of label and value pairs.</summary>
        static member Radar : dataset:System.IObservable<string * float> -> Charts.RadarChart
            
        /// <summary>Creates a new radar chart from a stream of values.</summary>
        /// <remakrs>The labels will be an auto-generated series from the index of the element.</remakrs>
        static member Radar : dataset:System.IObservable<float> -> Charts.RadarChart
    end
