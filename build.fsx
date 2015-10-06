#load "tools/includes.fsx"

open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.Charting")
        .VersionFrom("WebSharper")
        .WithFSharpVersion(FSharpVersion.FSharp30)
        .WithFramework(fun fw -> fw.Net40)

let main =
    bt.WebSharper.Library("WebSharper.Charting")
        .SourcesFromProject()
        .References(fun r ->
            [ 
                r.NuGet("WebSharper.ChartJs").Reference() 
                r.NuGet("IntelliFactory.Reactive").Reference() 
                r.NuGet("WebSharper.UI.Next").Reference()
            ])

let test =
    bt.WebSharper.Library("WebSharper.Charting.Test")
        .References(fun r ->
            [ 
                r.NuGet("WebSharper.ChartJs").Reference() 
                r.NuGet("IntelliFactory.Reactive").Reference() 
                r.NuGet("WebSharper.UI.Next").Reference()
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
