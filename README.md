# WebSharper.WebSpeech

[![NuGet](https://img.shields.io/nuget/v/WebSharper.WebSpeech.svg)](https://www.nuget.org/packages/WebSharper.WebSpeech)

WebSharper bindings for the browser [Web Speech API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Speech_API) —
both **speech synthesis** (text-to-speech) and **speech recognition** (speech-to-text) — for F# and C# web apps.

This repository contains:

| Project | Purpose |
|---|---|
| `WebSharper.WebSpeech` | The binding: typed F#/C# wrappers for the Web Speech API. |
| `WebSharper.WebSpeech.Extensions` | A small extension assembly that exposes `JS.Window.SpeechSynthesis` (the API's entry point lives on `window`, which the core binding cannot model on its own). |
| `WebSharper.WebSpeech.Sample` | A runnable WebSharper 10 SPA demonstrating both directions. |

Both assemblies ship in the single `WebSharper.WebSpeech` NuGet package.

## Live demos

- **Binding sample (this repo):** https://dotnet-websharper.github.io/webspeech/
- **Sample in the WebSharper samples gallery:** https://websharper-samples.github.io/WebSpeech/
- **Sample source in the gallery:** https://github.com/websharper-samples/WebSpeech
- **WebSharper:** https://websharper.com

> Speech features require a Chromium-based browser (Chrome/Edge) for recognition;
> synthesis works in most modern browsers. Recognition also needs microphone
> permission and a secure context (`https://` or `localhost`).

## Install

```bash
dotnet add package WebSharper.WebSpeech
```

Or with Paket:

```fsharp
nuget WebSharper.WebSpeech
```

The package targets `netstandard2.0` and works with any WebSharper 10 project.

## Usage

All types live in the `WebSharper.WebSpeech` namespace. The API surface mirrors the
browser spec closely, so MDN examples translate almost one-to-one.

### Text to speech

```fsharp
open WebSharper
open WebSharper.JavaScript
open WebSharper.WebSpeech

[<JavaScript>]
let Speak (text: string) =
    let utterance = SpeechSynthesisUtterance(text)
    utterance.Lang <- "en-US"
    utterance.Rate <- 1.0      // 0.1 .. 10
    utterance.Pitch <- 1.0     // 0 .. 2
    utterance.Volume <- 1.0    // 0 .. 1
    JS.Window.SpeechSynthesis.Speak(utterance)
```

`JS.Window.SpeechSynthesis` comes from the Extensions assembly (included in the package).

### Speech to text

```fsharp
open WebSharper
open WebSharper.JavaScript
open WebSharper.WebSpeech

[<JavaScript>]
let Listen (onText: string -> unit) =
    let recognition = SpeechRecognition()   // falls back to webkitSpeechRecognition
    recognition.Lang <- "en-US"
    recognition.Continuous <- false
    recognition.InterimResults <- false

    recognition.Onresult <- fun event ->
        // Results is an ArrayLike of ArrayLike alternatives.
        let alternative = event.Results.Item(0).Item(0)
        onText alternative.Transcript

    recognition.Onerror <- fun event ->
        Console.Error($"Speech recognition error: {event.Error}")

    recognition.Start()
```

### Notes on the API

- `SpeechRecognition()` uses `window.SpeechRecognition || window.webkitSpeechRecognition`
  under the hood, so it works in Chrome/Edge without feature detection in your app.
- `SpeechRecognitionErrorEvent.Error` is a typed enum (`SpeechRecognitionErrorEvent.ErrorCode`), not a string.
- Newer spec members are bound too: `SpeechRecognition.ProcessLocally`,
  `SpeechRecognition.Phrases` (with `SpeechRecognitionPhrase`), and `SpeechRecognition.MaxAlternatives`.
- Recognition results are nested `ArrayLike` collections; index them with `.Item(i)`.

## Build from source

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 9.0 or later (the sample targets `net9.0`).
- [Node.js](https://nodejs.org/) (LTS) for the esbuild bundle step.
- [Paket](https://fsprojects.github.io/Paket/) as a local dotnet tool (already configured in `.config/dotnet-tools.json`).

### Restore and build

```bash
dotnet tool restore
dotnet tool run paket restore
dotnet build SpeechAPI.sln -c Release
```

### Run the sample

```bash
cd WebSharper.WebSpeech.Sample
dotnet run
```

Then open the URL printed in the console (see `Properties/launchSettings.json`;
by default `https://localhost:52957`). In Release the sample runs the esbuild
bundle step automatically; in Debug it serves the WebSharper output directly.

### Pack the NuGet package

After a Release build (so the assemblies exist in `bin/Release/netstandard2.0`):

```bash
dotnet tool run paket pack ./out
```

The resulting `.nupkg` contains both `WebSharper.WebSpeech.dll` and
`WebSharper.WebSpeech.Extensions.dll` under `lib/netstandard2.0`, plus the binding source.

### Deployment

Pushing to `master` triggers `.github/workflows/ghpages.yml`, which builds the
sample in Release and publishes `WebSharper.WebSpeech.Sample/wwwroot` to the
`gh-pages` branch via GitHub Pages.

## Repository layout

```
WebSharper.WebSpeech/            # the binding
  Main.fs                        # types + [<assembly: Extension>] registration
WebSharper.WebSpeech.Extensions/ # Window.SpeechSynthesis extension
  Library.fs
WebSharper.WebSpeech.Sample/     # WS10 SPA sample
  Client.fs
  wwwroot/index.html             # template (ws-onclick / ws-var / ws-hole)
  esbuild.config.mjs
nuget/                           # Paket pack template
.github/workflows/ghpages.yml    # build + deploy sample to gh-pages
```

## License

Apache 2.0 — see [LICENSE.md](LICENSE.md).
