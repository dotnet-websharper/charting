namespace WebSharper.Charting

module Pervasives = begin
    
    type Size = | Size of width: int * height: int
    
    /// Represents color values in 3 different notation.
    [<RequireQualifiedAccessAttribute ()>]
    type Color =
        | Rgba of red: int * green: int * blue: int * alpha: float
        | Hex  of string
        | Name of string
        
        with override ToString : unit -> string
end
