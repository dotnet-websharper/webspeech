namespace SpeechAPI

open IntelliFactory.WebSharper.InterfaceGenerator

module Definition =
    open IntelliFactory.WebSharper.Dom
    open IntelliFactory.WebSharper.Html5

    let O = T<unit>
    let Ulong =  T<uint32>
    let Event = T<Event>

//    let SpeechGrammarList = Type.New ()
    let SpeechGrammar = Type.New ()
    let ErrorCode = Type.New ()
    let SpeechRecognitionEvent = Type.New ()
    let SpeechRecognitionError = Type.New ()

    let SpeechRecognition =
        Class "SpeechRecognition"
        |=> Inherits T<EventTarget>
        |+> [ 
            Constructor O 
            |> WithInline "new (window.SpeecRecognition || window.webkitSpeechRecognition)()"
        ]
        |+> Protocol [
            "grammars" =@ Type.ArrayOf SpeechGrammar
            |> WithComment "Stores the collection of SpeechGrammar objects which represent the grammars that are active for this recognition."
            "lang" =@ T<string>
            |> WithComment "This attribute will set the language of the recognition for the request, using a valid BCP 47 language tag."
            "continuous" =@ T<bool>
            |> WithComment "Indicates whether the object is one-shot only or does continously recotd and recognize speech."
            "interimResults" =@ T<bool>
            |> WithComment "Controls whether interim results are returned."
            "maxAlternatives" =@ Ulong
            |> WithComment "Sets the maximum number of SpeechRecognitionAlternatives per result."
            "serviceURI" =@ T<string>
            |> WithComment "The serviceURI attribute specifies the location of the speech recognition service that the web application wishes to use."

            "start" => O ^-> O
            |> WithComment "When the start method is called it represents the moment in time the web application wishes to begin recognition."
            "stop" => O ^-> O
            |> WithComment "The stop method represents an instruction to the recognition service to stop listening to more audio, \
                            and to try and return a result using just the audio that it has already received for this recognition."
            "abort" => O ^-> O
            |> WithComment "The abort method is a request to immediately stop listening and stop recognizing and do not return any information but that the system is done."

            "onaudiostart" =@ Event ^-> O
            |> WithComment "Fired when the user agent has started to capture audio."
            "onsoundstart" =@ Event ^-> O
            |> WithComment "Fired when some sound, possibly speech, has been detected."
            "onspeechstart" =@ Event ^-> O
            |> WithComment "Fired when the speech that will be used for speech recognition has started."
            "onspeechend" =@ Event ^-> O
            |> WithComment "Fired when the speech that will be used for speech recognition has ended."
            "onsoundend" =@ Event ^-> O
            |> WithComment "Fired when some sound is no longer detected. "
            "onaudioend" =@ Event ^-> O
            |> WithComment "Fired when the user agent has finished capturing audio."
            "onresult" =@ SpeechRecognitionEvent ^-> O
            |> WithComment "Fired when the speech recognizer returns a result."
            "onnomatch" =@ SpeechRecognitionEvent ^-> O
            |> WithComment "Fired when the speech recognizer returns a final result with no recognition hypothesis that meet or exceed the confidence threshold."
            "onerror" =@ SpeechRecognitionError ^-> O
            |> WithComment "Fired when a speech recognition error occurs."
            "onstart" =@ Event ^-> O
            |> WithComment "Fired when the recognition service has begun to listen to the audio with the intention of recognizing."
            "onend" =@ Event ^-> O
            |> WithComment "Fired when the service has disconnected."
        ]

    let SpeechRecognitionErrorClass =
        Class "SpeechRecognitionError"
        |=> SpeechRecognitionError
        |=> Inherits Event
        |=> Nested [
            Pattern.EnumStrings "ErrorCode" [
                "no-speech"
                "aborted"
                "audio-capture"
                "network"
                "not-allowed"
                "service-not-allowed"
                "bad-grammar"
                "language-not-supported"
            ]
            |=> ErrorCode
        ]
        |+> Protocol [
            "error" =? ErrorCode
            "message" =? T<string>
        ]

    let SpeechRecognitionAlternative =
        Class "SpeechRecognitionAlternative"
        |+> Protocol [
            "transcript" =? T<string>
            |> WithComment "The transcript string represents the raw words that the user spoke." 
            "confidence" =? T<float>
            |> WithComment "The confidence represents a numeric estimate between 0 and 1 of how confident the recognition system is that the recognition is correct."
        ]

//    let SpeechRecognitionResult = 
//        Class "SpeechRecognitionResult"
//        |+> Protocol [
//            "length" =? Ulong
//            "item" => Ulong?index ^-> SpeechRecognitionAlternative
//            "final" =? T<bool>
//        ]

//    let SpeechRecognitionResultList =
//        Class "SpeechRecognitionResultList"
//        |=> Inherits (Type.ArrayOf SpeechRecognitionResult)
//        |+> Protocol [
//            "length" =? Ulong
//            "item" => Ulong?index ^-> SpeechRecognitionResult
//        ]

    let SpeechRecognitionEventClass =
        Class "SpeechRecognitionEvent"
        |=> SpeechRecognitionEvent
        |=> Inherits Event
        |+> Protocol [
            "resultIndex" =? Ulong
            |> WithComment "The lowest index in the results array that has changed."
            "results" =? Type.ArrayOf (Type.ArrayOf SpeechRecognitionAlternative)
            |> WithComment "The array of all current recognition results for this session. \
                            Specifically all final results that have been returned, followed by the current best hypothesis for all interim results."
            "interpretation" =? T<obj>
            |> WithComment "The interpretation represents the semantic meaning from what the user said."
            "emma" =? T<Document>
            |> WithComment "EMMA 1.0 representation of this result."
        ]

    let SpeechGrammarClass =
        Class "SpeechGrammar"
        |=> SpeechGrammar
        |+> [ Constructor O ]
        |+> Protocol [
            "src" =? T<string>
            |> WithComment "The required src attribute is the URI for the grammar."
            "weight" =? T<float>
            |> WithComment "The optional weight attribute controls the weight that the speech recognition service should use with this grammar."
        ]

//    let SpeechGrammarListClass =
//        Class "SpeechGrammarList"
//        |=> SpeechGrammarList
//        |=> Inherits (Type.ArrayOf SpeechGrammar)
//        |+> [ Constructor O ]
//        |+> Protocol [
//            "length" =? Ulong
//            "item" => Ulong?index ^-> SpeechGrammar
//            "addFromURI" => (T<string>?src * !? T<float>?weight) ^-> O
//            "addFromString" => (T<string>?src * !? T<float>?wight) ^-> O
//        ]

    let SpeechSynthesisUtterance = Type.New ()
//    let SpeechSynthesisVoiceList = Type.New ()
    let SpeechSynthesisVoice = Type.New ()

    let SpeechSynthesis =
        Class "SpeechSynthesis"
        |+> Protocol [
            "pending" =? T<bool>
            |> WithComment "This attribute is true if the queue for the global SpeechSynthesis instance contains any utterances which have not started speaking."
            "speaking" =? T<bool>
            |> WithComment "This attribute is true if an utterance is being spoken."
            "paused" =? T<bool>
            |> WithComment "This attribute is true when the global SpeechSynthesis instance is in the paused state."

            "speak" => SpeechSynthesisUtterance?utterance ^-> O
            |> WithComment "This method appends the SpeechSynthesisUtterance object to the end of the queue for the global SpeechSynthesis instance."
            "cancel" => O ^-> O
            |> WithComment "This method removes all utterances from the queue. If an utterance is being spoken, speaking ceases immediately."
            "pause" => O ^-> O
            |> WithComment "This method puts the global SpeechSynthesis instance into the paused state. If an utterance was being spoken, it pauses mid-utterance."
            "resume" => O ^-> O
            |> WithComment "This method puts the global SpeechSynthesis instance into the non-paused state."

            "getVoices" => O ^-> Type.ArrayOf SpeechSynthesisVoice
            |> WithComment "This method returns the available voices."
        ]

    let SpeechSynthesisGetter =
        Class "window"
        |+> [
            "speechSynthesis" =? SpeechSynthesis
        ]

    let SpeechSynthesisEvent =
        Class "SpeechSynthesisEvent"
        |=> Event
        |+> Protocol [
            "charIndex" =? Ulong
            |> WithComment "This attribute indicates the zero-based character index into the original utterance string \
                            that most closely approximates the current speaking position of the speech engine."
            "elapsedTime" =? T<float>
            |> WithComment "This attribute indicates the time, in seconds, that this event triggered, relative to when this utterance has begun to be spoken. "
            "name" =? T<string>
            |> WithComment "For mark events, this attribute indicates the name of the marker, as defined in SSML as the name attribute of a mark element."
        ]

    let SpeechSynthesisUtteranceClass =
        Class "SpeechSynthesisUtterance"
        |=> SpeechSynthesisUtterance
        |=> Inherits T<EventTarget>
        |+> [
            Constructor O
            Constructor T<string>?text
        ]
        |+> Protocol [
            "text" =@ T<string>
            |> WithComment "This attribute specifies the text to be synthesized and spoken for this utterance."
            "lang" =@ T<string>
            |> WithComment "This attribute specifies the language of the speech synthesis for the utterance, using a valid BCP 47 language tag."
            "voiceURI" =@ T<string>
            |> WithComment "The voiceURI attribute specifies speech synthesis voice and the location of the speech synthesis service that the web application wishes to use."
            "volume" =@ T<float>
            |> WithComment "This attribute specifies the speaking volume for the utterance."
            "rate" =@ T<float>
            |> WithComment "This attribute specifies the speaking rate for the utterance."
            "pitch" =@ T<float>
            |> WithComment "This attribute specifies the speaking pitch for the utterance."

            "onstart" =@ SpeechSynthesisEvent ^-> O
            |> WithComment "Fired when this utterance has begun to be spoken."
            "onend" =@ SpeechSynthesisEvent ^-> O
            |> WithComment "Fired when this utterance has completed being spoken."
            "onerror" =@ SpeechSynthesisEvent ^-> O
            |> WithComment "Fired if there was an error that prevented successful speaking of this utterance."
            "onpause" =@ SpeechSynthesisEvent ^-> O
            |> WithComment "Fired when and if this utterance is paused mid-utterance."
            "onresume" =@ SpeechSynthesisEvent ^-> O
            |> WithComment "Fired when and if this utterance is resumed after being paused mid-utterance."
            "onmark" =@ SpeechSynthesisEvent ^-> O
            |> WithComment "Fired when the spoken utterance reaches a named mark tag in SSML."
            "onboundary" =@ SpeechSynthesisEvent ^-> O
            |> WithComment "Fired when the spoken utterance reaches a word or sentence boundary."
        ]

    let SpeechSynthesisVoiceClass =
        Class "SpeechSynthesisVoice"
        |=> SpeechSynthesisVoice
        |+> Protocol [
            "voiceURI" =? T<string>
            |> WithComment "The voiceURI attribute specifies the speech synthesis voice and the location of the speech synthesis service for this voice."
            "name" =? T<string>
            |> WithComment "This attribute is a human-readable name that represents the voice."
            "lang" =? T<string>
            |> WithComment "This attribute is a BCP 47 language tag indicating the language of the voice."
            "localService" =? T<bool>
            |> WithComment "This attribute is true for voices supplied by a local speech synthesizer, and is false for voices supplied by a remote speech synthesizer service."
            "default" =? T<bool>
            |> WithComment "Indicates whether the language is the default voice of the given language."

        ]
    
//    let SpeechSynthesisVoiceListClass =
//        Class "SpeechSynthesisVoiceList"
//        |=> SpeechSynthesisVoiceList
//        |+> [ Constructor O ]
//        |+> Protocol [
//            "length" =? Ulong
//            "item" => Ulong?index ^-> SpeechSynthesisVoice
//        ]
    

    let Assembly =
        Assembly [
            Namespace "IntelliFactory.WebSharper.Html5" [
                SpeechRecognition
                SpeechRecognitionErrorClass
                SpeechRecognitionAlternative
//                SpeechRecognitionResult
//                SpeechRecognitionResultList
                SpeechRecognitionEventClass
                SpeechGrammarClass
//                SpeechGrammarListClass
                SpeechSynthesis
                SpeechSynthesisGetter
                SpeechSynthesisEvent
                SpeechSynthesisUtteranceClass
                SpeechSynthesisVoiceClass
//                SpeechSynthesisVoiceListClass
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()