namespace WebSharper.Charting

[<AutoOpen>]
module Pervasives = begin
    type Size = | Size of width: int * height: int
    
    [<RequireQualifiedAccessAttribute>]
    type Color =
        | Rgba of red: int * green: int * blue: int * alpha: float
        | Hex  of string
        | Name of string
        
        with override ToString : unit -> string

    module internal Seq = begin
        val headOption : seq<'a> -> option<'a>
    end

    module internal Reactive = begin
        val SequenceOnlyNew : seq<#System.IObservable<'a>> -> System.IObservable<seq<'a>>
    end

    val internal streamWithLabel : System.IObservable<float> -> System.IObservable<string * float>
    val internal withIndex : seq<'T> -> seq<string * 'T>
end
