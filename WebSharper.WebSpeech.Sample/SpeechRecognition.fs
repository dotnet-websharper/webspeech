namespace Site

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.JavaScript

[<JavaScript>]
module SpeechRecognition =
    module Utils =
        let toArray(a:ArrayLike<_>) =
            if a.Length <= 0 
            then [||] 
            else [| for i in 0..(a.Length-1) -> a[i] |]
    let MaxConfidence = Array.maxBy (fun (e:SpeechRecognitionAlternative) -> e.Confidence)
    let inline GetTranscript (e:SpeechRecognitionAlternative) = e.Transcript
    let punctuatorUrl = "http://bark.phon.ioc.ee/punctuator"

    [<Inline "$request.send($data)">]
    let SendRequest (request:XMLHttpRequest) (data:string) = X<unit>

    let Punctuate (text:string):Async<string> =
        Async.FromContinuations 
            <| fun (ok, ko, _) -> 
                let req = new XMLHttpRequest()
                req.Open("POST",punctuatorUrl,true)
                req.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded")
                req.Onload <- (fun _ -> ok (req.ResponseText + (if req.ResponseText.EndsWith(".") then "" else ".")))
                req.Onerror <- (fun _ -> ko <| new System.Exception(req.ResponseText))
                SendRequest req <| sprintf "text=%s" text

    let speechRecognize (var: Var<string>) (persistentResults: Var<string list>) =
        SpeechRecognition(
            Lang = "en-US",
            Continuous = true,
            Onend = (fun _ -> 
                var.View
                |> View.MapAsync (fun res -> async {return! Punctuate res})
                |> View.Get (fun res -> 
                    Var.Update persistentResults (fun pr -> pr@[res])
                )
            ),
            Onresult = (fun res -> 
                res.Results
                |> Utils.toArray
                |> Array.map (Utils.toArray 
                    >> MaxConfidence
                    >> GetTranscript)
                |> String.concat " "
                |> var.Set
            )
        )