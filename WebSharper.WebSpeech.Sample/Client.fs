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

    [<SPAEntryPoint>]
    let Main() =
        //All.Show()
        let shownType = Var.Create<SampleTypes option> None
        
        let navChild title sampleType = 
            shownType.View 
            |> Doc.BindView(fun t -> 
                li [
                    let isActive = t = Some sampleType
                    if isActive 
                    then attr.``class`` "active"
                    else on.click (fun _ _ -> shownType.Set <| Some sampleType)
                ] [text title])

        Doc.Concat [
            navChild "Speech Synthesis" SampleTypes.SpeechSynthesis
            navChild "Speech Utterance" SampleTypes.SpeechUtterance
        ] 
        |> Doc.RunById("sample-navs")

        Templates.Index.SpeechSide()
            .GitHubUrl(shownType.View.Map(fun st ->
                let fileName = 
                    match st with
                    | Some s -> sprintf "%A.fs" s
                    | None -> "Client.fs"
                $"http://github.com/intellifactory/websharper.webspeech/blob/master/Site/{fileName}" 
            )).Doc() 
        |> Doc.RunById "sample-side"

        shownType.View
        |> Doc.BindView (function
                | Some SpeechSynthesis ->
                    let txt = Var.Create ""
                    let isPlaying = Var.Create false
                    let speechRecog = SpeechSynthesis.SpeechRecognize txt
                    Templates.Index.SpeechSynthesis()
                        .StartAttr(
                            attr.disabledBool (isPlaying.View)
                        )
                        .StopAttr(
                            attr.disabledBool (isPlaying.View.Map(not))
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
                | Some SpeechUtterance -> 
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
                | _ -> div [] []
            ) 
            |> Doc.RunById "sample-main"
            
            
            