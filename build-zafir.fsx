#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("Zafir.WebSpeech")
        .VersionFrom("Zafir")
        .WithFSharpVersion(FSharpVersion.FSharp30)
        .WithFramework(fun fw -> fw.Net40)

let main =
    (bt.Zafir.Extension("WebSharper.WebSpeech")
    |> FSharpConfig.BaseDir.Custom "WebSpeech")
        .SourcesFromProject("WebSpeech.fsproj")

(*let test =
    (bt.WebSharper.BundleWebsite("IntelliFactory.WebSharper.WebRTC.Tests")
    |> FSharpConfig.BaseDir.Custom "Tests")
        .SourcesFromProject("Tests.fsproj")
        .References(fun r -> [r.Project main])*)

bt.Solution [
    main
    //test

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c with
                Title = Some "Zafir.WebSpeech"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://bitbucket.org/intellifactory/websharper.webspeech"
                Description = "WebSharper Extensions for WebSpeech"
                Authors = ["IntelliFactory"]
                RequiresLicenseAcceptance = true })
        .Add(main)

]
|> bt.Dispatch
