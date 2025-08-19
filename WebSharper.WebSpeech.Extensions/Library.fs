namespace WebSharper.WebSpeech

open WebSharper
open WebSharper.JavaScript

[<JavaScript; AutoOpen>]
module Extensions =

    type Window with
        [<Inline "$this.speechSynthesis">]
        member this.Speech