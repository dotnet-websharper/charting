#load "tools/includes.fsx"

open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.Charting")
        .VersionFrom("WebSharper")
        .WithFramework(fun fw -> fw.Net40)

let main =
    bt.WebSharper4.Library("WebSharper.Charting")
        .SourcesFromProject()
        .WithSourceMap()
        .References(fun r ->
            [ 
                r.NuGet("WebSharper.ChartJs").Latest(true).ForceFoundVersion().Reference()
                r.NuGet("WebSharper.Reactive").Latest(true).ForceFoundVersion().Reference()
                r.NuGet("WebSharper.UI.Next").Latest(true).ForceFoundVersion().Reference()
            ])

let test =
    bt.WebSharper4.Library("WebSharper.Charting.Test")
        .WithSourceMap()
        .References(fun r ->
            [ 
                r.NuGet("WebSharper.ChartJS").Latest(true).ForceFoundVersion().Reference()
                r.NuGet("WebSharper.Reactive").Latest(true).ForceFoundVersion().Reference()
                r.NuGet("WebSharper.UI.Next").Latest(true).ForceFoundVersion().Reference()
                r.Project main
            ])

bt.Solution [
    main
    test

    bt.NuGet.CreatePackage()
        .Configure(fun configuration ->
            { configuration with
                Title = Some "WebSharper.Charting"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://github.com/intellifactory/websharper.charting"
                Description = "Chart combinator-library for WebSharper"
                Authors = [ "IntelliFactory" ]
                RequiresLicenseAcceptance = true })
        .Add(main)
]
|> bt.Dispatch
