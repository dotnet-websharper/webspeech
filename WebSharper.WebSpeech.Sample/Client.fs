namespace WebSharper.WebSpeech.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.WebSpeech

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let output = Var.Create ""
    let textInput = Var.Create "Hello, I can talk using Web Speech API!"
    
    [<SPAEntryPoint>]
    let Main () =
        let recognition = new SpeechRecognition()
        recognition.Lang <- "en-US"
        recognition.Continuous <- false
        recognition.InterimResults <- false

        IndexTemplate.Main()
            .PageInit(fun () -> 
                recognition.Onresult <- fun event -> 
                    let speechRecognitionAlternative = event.Results[0][0]
                    let transcript = speechRecognitionAlternative.Transcript
                    output := transcript
                    Console.Log($"output {output.Value}")

                recognition.Onerror <- fun event ->
                    Console.Error $"Speech recognition error: {event.Error}"                
            )
            .StartBtn(fun _ -> recognition.Start())
            .SpeakBtn(fun _ -> 
                Console.Log($"textInput {textInput.Value}")
                
                let utterance = new SpeechSynthesisUtterance(textInput.Value)
                utterance.Lang <- "en-US"
               
                JS.Window.SpeechSynthesis.Speak(utterance)
            )
            .Output(output.V)
            .TextInput(textInput.V)
            .Doc()
        |> Doc.RunById "main"
