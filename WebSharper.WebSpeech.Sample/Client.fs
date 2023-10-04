namespace Site

open WebSharper

module Templates =
    open WebSharper.UI.Templating
    type Index = Template<"index.html", clientLoad=ClientLoad.FromDocument>

[<JavaScript>]
module Client =
    open WebSharper.UI
    open WebSharper.UI.Html
    open WebSharper.UI.Client
    open WebSharper.JavaScript

    type SampleTypes =
    | SpeechSynthesis
    | SpeechUtterance
    | SpeechRecognition

    [<SPAEntryPoint>]
    let Main() =
        //All.Show()
        let shownType = Var.Create<SampleTypes> SpeechSynthesis
        
        let navChild title sampleType = 
            shownType.View 
            |> Doc.BindView(fun t -> 
                li [
                    let isActive = t = sampleType
                    if isActive 
                    then attr.``class`` "active"
                    else on.click (fun _ _ -> shownType.Set <| sampleType)
                ] [a [attr.href "#"] [text title]])

        Doc.Concat [
            navChild "Speech Synthesis" SampleTypes.SpeechSynthesis
            navChild "Speech Utterance" SampleTypes.SpeechUtterance
            navChild "Speech Recognition+Punctuator" SampleTypes.SpeechRecognition
        ] 
        |> Doc.RunById("sample-navs")

        Templates.Index.SpeechSide()
            .GitHubUrl(shownType.View.Map(fun st ->
                let fileName = sprintf "%A.fs" st
                $"http://github.com/intellifactory/websharper.webspeech/blob/master/Site/{fileName}" 
            )).Doc() 
        |> Doc.RunById "sample-side"

        shownType.View
        |> Doc.BindView (function
                | SpeechSynthesis ->
                    let txt = Var.Create ""
                    let isPlaying = Var.Create false
                    let speechRecog = SpeechSynthesis.SpeechRecognize txt
                    Templates.Index.SpeechSynthesis()
                        .StartAttr(
                            attr.disabledBool isPlaying.View
                        )
                        .StopAttr(
                            attr.disabledBool <| isPlaying.View.Map(not)
                        )
                        .OnStart(fun _ -> 
                            speechRecog.Start()
                            isPlaying.Set true
                        )
                        .OnStop(fun _ ->
                            speechRecog.Stop()
                            isPlaying.Set false
                        )
                        .PreText(txt.View)
                        .Doc()
                | SpeechUtterance -> 
                    Templates.Index.SpeechUtterance()
                        .UtteranceSamples(
                            SpeechUtterance.Texts 
                            |> List.map (fun (txt,lang) -> 
                                let var = Var.Create txt
                                let utterance = JavaScript.SpeechSynthesisUtterance(var.Value)

                                Templates.Index.UtteranceField()
                                    .Text(var)  
                                    .OnSpeak(fun _ -> 
                                        utterance.Text <- var.Value
                                        utterance.Lang <- lang
                                        JavaScript.Window.SpeechSynthesis.Speak(utterance)
                                    )
                                    .Doc()
                                        
                                )
                            |> Doc.Concat
                        )
                        .Doc()
                | SpeechRecognition ->
                    let isRunning = Var.Create false
                    let intermediateResult = Var.Create ""
                    let recognizerResult = Var.Create<string list> []
                    let recognizer = SpeechRecognition.speechRecognize intermediateResult recognizerResult

                    let isDisabledDynPred(invert:bool) = 
                        attr.disabledDynPred 
                        <|| (isRunning.View.Map(function | true -> "disabled" | _ -> ""), isRunning.View|> if invert then View.Map(not) else id)
                    
                    Templates.Index.SpeechRecognition()
                        .Start(fun _ -> 
                            isRunning.Set true
                            recognizer.Start()
                        )
                        .End(fun _ -> 
                            isRunning.Set false
                            recognizer.Stop()
                        )
                        .IsActive(isRunning.View.Map(function | true -> "pulse" | _ -> ""))
                        .StartAttr(isDisabledDynPred(false))
                        .EndAttr(isDisabledDynPred(true))
                        .WithoutPunctuation(intermediateResult.View.Doc text)
                        .WithPunctuation(recognizerResult.View.DocSeqCached(fun p ->
                            div [attr.``class`` "result"] [text p]
                        ))
                        .Doc()
            ) 
            |> Doc.RunById "sample-main"
            
            
            