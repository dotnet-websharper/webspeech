namespace WebSharper.WebSpeech.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.WebSpeech

[<JavaScript>]
module Client =
    // The template is loaded from the DOM, so you can edit wwwroot/index.html
    // and refresh your browser without recompiling (unless you add/remove holes).
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    [<SPAEntryPoint>]
    let Main () =
        let output = Var.Create ""
        let textInput = Var.Create "Hello, I can talk using the Web Speech API!"

        let recognition = SpeechRecognition()
        recognition.Lang <- "en-US"
        recognition.Continuous <- false
        recognition.InterimResults <- false

        recognition.Onresult <- fun event ->
            // event.Results is an ArrayLike of ArrayLike alternatives.
            let alternative = event.Results.Item(0).Item(0)
            output.Value <- alternative.Transcript
            Console.Log($"output {output.Value}")

        recognition.Onerror <- fun event ->
            Console.Error($"Speech recognition error: {event.Error}")

        IndexTemplate()
            .StartBtn(fun _ -> recognition.Start())
            .SpeakBtn(fun _ ->
                let utterance = SpeechSynthesisUtterance(textInput.Value)
                utterance.Lang <- "en-US"
                JS.Window.SpeechSynthesis.Speak(utterance))
            .Output(output.View)
            .TextInput(textInput)
            .Bind()
