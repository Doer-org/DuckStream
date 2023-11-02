module Tests

open Xunit
open FsHttp
open System.Net
open FsUnit.Xunit
open Microsoft.AspNetCore.Mvc.Testing

let webapp = new WebApplicationFactory<Program.Program>()
let client = webapp.CreateClient()

[<Fact>]
let health () =

    let resp =
        client.GetAsync("/health")
        |> Async.AwaitTask
        |> Async.RunSynchronously

    resp.StatusCode |> should equal HttpStatusCode.OK
