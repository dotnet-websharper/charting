
#r "nuget: FAKE.Core"
#r "nuget: Fake.Core.Target"
#r "nuget: Fake.IO.FileSystem"
#r "nuget: Fake.Tools.Git"
#r "nuget: Fake.DotNet.Cli"
#r "nuget: Fake.DotNet.AssemblyInfoFile"
#r "nuget: Fake.DotNet.Paket"
#r "nuget: Paket.Core, 8.1.0-alpha004"
#r "nuget: MSBuild.StructuredLogger"

open Fake.Core
System.Environment.GetCommandLineArgs()
|> Array.skip 2 // skip fsi.exe; build.fsx
|> Array.toList
|> Fake.Core.Context.FakeExecutionContext.Create false __SOURCE_FILE__
|> Fake.Core.Context.RuntimeContext.Fake
|> Fake.Core.Context.setExecutionContext


#load "paket-files/wsbuild/github.com/dotnet-websharper/build-script/WebSharper.Fake.fsx"
open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open WebSharper.Fake

let targets =
    WSTargets.Default (fun () -> GetSemVerOf "WebSharper" |> ComputeVersion)
    |> fun args ->
        { args with
            Attributes =
                    [
                        AssemblyInfo.Company "IntelliFactory"
                        AssemblyInfo.Copyright "(c) IntelliFactory 2023"
                        AssemblyInfo.Title "https://github.com/dotnet-websharper/charting"
                        AssemblyInfo.Product "WebSharper Charting"
                    ]
        }
    |> MakeTargets

Target.runOrDefault "Build"
