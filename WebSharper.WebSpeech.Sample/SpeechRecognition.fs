namespace WebSharper.WebSpeech.Sample

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.JavaScript

//module Templates =
//    type MainTemplate = Template<"notsoindexahaha.html", ClientLoad.FromDocument>

[<JavaScript>]
module Client =
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
    //[<SPAEntryPoint>]
    //let Main () =
    //    let isRunning = Var.Create false
    //    let intermediateResult = Var.Create ""
    //    let recognizerResult = Var.Create<string list> []
    //    let recognizer = speechRecognize intermediateResult recognizerResult

    //    let isDisabledDynPred(invert:bool) = 
    //        attr.disabledDynPred 
    //        <|| (isRunning.View.Map(function | true -> "disabled" | _ -> ""), isRunning.View|> if invert then View.Map(not) else id)

    //    Templates.MainTemplate
    //        .Main()
    //        .Start(fun _ -> 
    //            isRunning.Set true
    //            recognizer.Start()
    //        )
    //        .End(fun _ -> 
    //            isRunning.Set false
    //            recognizer.Stop()
    //        )
    //        .IsActive(isRunning.View.Map(function | true -> "pulse" | _ -> ""))
    //        .StartAttr(isDisabledDynPred(false))
    //        .EndAttr(isDisabledDynPred(true))
    //        .WithoutPunctuation(intermediateResult.View.Doc text)
    //        .WithPunctuation(recognizerResult.View.DocSeqCached(fun p ->
    //            div [attr.``class`` "result"] [text p]
    //        ))
    //        .Doc()
    //        |> Doc.RunById "main"