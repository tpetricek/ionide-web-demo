#nowarn "40"
#I "packages/FAKE/tools"
#I "packages/Suave/lib/net40"
#r "FakeLib.dll"
#r "Suave.dll"
open Fake
open System
open Suave.Web
open Suave.Http
open Suave.Http.Files

let startWebServer port =
    let defaultBinding = defaultConfig.bindings.[0]
    let withPort = { defaultBinding.socketBinding with port = uint16 port }
    let serverConfig =
        { defaultConfig with
            bindings = [ { defaultBinding with socketBinding = withPort } ]
            homeFolder = Some __SOURCE_DIRECTORY__ }
    let app =
        Writers.setHeader "Cache-Control" "no-cache, no-store, must-revalidate"
        >>= Writers.setHeader "Pragma" "no-cache"
        >>= Writers.setHeader "Expires" "0"
        >>= Successful.OK "Hello!"
    startWebServerAsync serverConfig app |> snd |> Async.RunSynchronously

Target "AutoStart" (fun _ ->
    startWebServer (int (getBuildParam "port"))
)

Target "Run" (fun _ ->
    startWebServer 8015
    Diagnostics.Process.Start "http://localhost:8015" |> ignore
)

RunTargetOrDefault "Run"