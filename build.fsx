#if INTERACTIVE
#r "nuget: FAKE.Core"
#r "nuget: Fake.Core.Target"
#r "nuget: Fake.IO.FileSystem"
#r "nuget: Fake.Tools.Git"
#r "nuget: Fake.DotNet.Cli"
#r "nuget: Fake.DotNet.AssemblyInfoFile"
#r "nuget: Fake.DotNet.Paket"
#r "nuget: Paket.Core"
#else
#r "paket:
nuget FAKE.Core
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget Fake.Tools.Git
nuget Fake.DotNet.Cli
nuget Fake.DotNet.AssemblyInfoFile
nuget Fake.DotNet.Paket
nuget Paket.Core //"
#endif

#load "paket-files/wsbuild/github.com/dotnet-websharper/build-script/WebSharper.Fake.fsx"
open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open WebSharper.Fake

let targets =
    WSTargets.Default (fun () -> GetSemVerOf "WebSharper" |> ComputeVersion)
    |> MakeTargets

let rec CopyDirectory source dest =
    let dif = System.IO.DirectoryInfo(source)
    System.IO.Directory.CreateDirectory dest
    for file in dif.GetFiles() do
        let targetPath = System.IO.Path.Combine(dest, file.Name)
        file.CopyTo targetPath
    for dir in dif.GetDirectories() do
        let newDir = System.IO.Path.Combine(dest, dir.Name)
        CopyDirectory dir.FullName newDir

Target.create "SampleBuild" <| fun o ->
    if System.IO.Directory.Exists "dist" then
        System.IO.Directory.Delete("dist", true)
    let di = System.IO.Directory.CreateDirectory "dist"
    System.IO.File.Copy("WebSharper.WebSpeech.Sample/index.html", "dist/index.html")
    System.IO.File.Copy("WebSharper.WebSpeech.Sample/Style.css", "dist/Style.css")
    CopyDirectory "WebSharper.WebSpeech.Sample/Content" "dist/Content"

"CI-Release" ==> "SampleBuild"

Target.runOrDefault "Build"
