#load "tools/includes.fsx"

open IntelliFactory.Build

let bt =
    BuildTool().PackageId("Zafir.Charting")
        .VersionFrom("Zafir")
        .WithFSharpVersion(FSharpVersion.FSharp30)
        .WithFramework(fun fw -> fw.Net40)

let main =
    bt.Zafir.Library("WebSharper.Charting")
        .SourcesFromProject()
        .References(fun r ->
            [ 
                r.NuGet("Zafir.ChartJs").ForceFoundVersion().Reference() 
                r.NuGet("Zafir.Reactive").ForceFoundVersion().Reference() 
                r.NuGet("Zafir.UI.Next").ForceFoundVersion().Reference()
            ])

let test =
    bt.Zafir.Library("WebSharper.Charting.Test")
        .References(fun r ->
            [ 
                r.NuGet("Zafir.ChartJs").Reference() 
                r.NuGet("Zafir.Reactive").Reference() 
                r.NuGet("Zafir.UI.Next").Reference()
                r.Project main
            ])

bt.Solution [
    main
    test

    bt.NuGet.CreatePackage()
        .Configure(fun configuration ->
            { configuration with
                Title = Some "Zafir.Charting"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://github.com/intellifactory/websharper.charting"
                Description = "Chart combinator-library for Zafir"
                Authors = [ "IntelliFactory" ]
                RequiresLicenseAcceptance = true })
        .Add(main)
]
|> bt.Dispatch
