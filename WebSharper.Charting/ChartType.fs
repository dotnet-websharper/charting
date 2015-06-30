namespace WebSharper.Charting

open WebSharper.JavaScript
open WebSharper.Html.Client

type GenericConfig<'T> =
    {
        DataSet : System.IObservable<'T>
        Width   : int
        Height  : int
    }

type LineChartConfig<'T> =
    {
        Generic : GenericConfig<'T>
        Window  : int
    }

type IChart<'T, 'U> =
    abstract member WithState : 'T -> #IChart<'T, 'U>
    abstract member AddData : 'T -> unit
    abstract member RemoveData : 'U -> unit

type IRenderer =
    abstract member Body : Dom.Node
    abstract member Render : unit -> unit

module ChartJs =
    open WebSharper.ChartJs

    let private getContext (canvas : Element) =
        (As<CanvasElement> canvas.Body).GetContext("2d")

    type RenderableChart(config : GenericConfig<_>, mk : CanvasRenderingContext2D -> ) =
        let canvas = Canvas []

        interface IRenderer with
            member x.Body = canvas.Body
            override x.Render () = 
                canvas.Body?width <- config.Width
                canvas.Body?height <- config.Height
                    
                x.WithContext <| getContext canvas

    type LineChart(context, lineChartData : LineChartConfig<_>) =
        

        let data =
            LineChartData(
                Labels   = Array.empty,
                Datasets =
                    [|LineChartDataset(Data = Array.empty)|])

        let options = 
            LineChartConfiguration(
                BezierCurve = false,
                DatasetFill = false)

        let chart = Chart(context).Line(data, options)


        interface IChart<string * float>
//            member x.AddData ((lbl, value)) = chart.AddData([|value|], lbl)
//            member x.RemoveData () = chart.RemoveData()

    type PieChart(context, ?data, ?options) =
        let data =
            defaultArg data
            <| [|PieChartDataset()|]

        let options = 
            defaultArg options
            <| PieChartConfiguration()

        let chart = Chart(context).Pie(data, options)
        let indicies = System.Collections.Generic.Queue<_>()

        interface IChart<int * PolarAreaChartDataset>
//            member x.AddData ((idx, data)) = 
//                chart.AddData(data, idx)
//                indicies.Enqueue idx
//            member x.RemoveData () = 
//                if indicies.Count > 0 then 
//                    chart.RemoveData <| indicies.Dequeue()
        

type ChartType =
    | Line
    | Pie
    | Doughnut
    | Radar