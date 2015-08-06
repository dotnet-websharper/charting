namespace WebSharper.Charting

/// Streams are representing the continuous flow of some type of data
/// that you are rendering on a specific chart.
/// They are buffered, which means they can only remember to the last n elements
/// up to the capacity.
type BufferedStream<'T> =
    class
        interface System.IObservable<'T>

        /// Creates a stream with the given capacity.
        new : capacity: int -> BufferedStream<'T>

        member Event : Event<'T>

        /// Puts a new value into the stream, then notifies every observer.
        member Trigger : v: 'T -> unit
    end
