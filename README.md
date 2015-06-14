# WebSharper.Charting
Charts for web applications

## Basic usage
```fsharp
seq { for x in -2.0 .. 2.0 do
    yield (string x, -1.0 * x**2.0)
}
|> Charting.LineChart id
|> Chart.WithDimension (300, 300)
|> Chart.WithTitle "f(x) = x * x"
|> (fun chart ->
  chart.appendTo "body"
)
```
The base `Chart` class inherits from `Pagelet`, so you can easily embed it to any HTML combinator or you can use its `AppendTo` function.
