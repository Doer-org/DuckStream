module Tests

open Xunit
open FsHttp
open System.Net
open FsUnit.Xunit
open Microsoft.AspNetCore.Mvc.Testing

open System.Text.Json

System.Environment.SetEnvironmentVariable("MOCK_DB", "true")
System.Environment.SetEnvironmentVariable("MOCK_ML", "true")
System.Environment.SetEnvironmentVariable("MOCK_GCS", "true")

let webapp = new WebApplicationFactory<Program.Program>()
let client = webapp.CreateClient()


[<Fact>]
let hello () =
    let resp = client.GetAsync("/") |> Async.AwaitTask |> Async.RunSynchronously

    let body =
        resp.Content.ReadAsStringAsync().Result
        |> JsonSerializer.Deserialize<{| hello: string |}>

    resp.StatusCode |> should equal HttpStatusCode.OK
    body |> should equal {| hello = "world" |}

[<Fact>]
let health () =
    let resp = client.GetAsync("/health") |> Async.AwaitTask |> Async.RunSynchronously

    let body =
        resp.Content.ReadAsStringAsync().Result
        |> JsonSerializer.Deserialize<{| client_url: string
                                         db: {| is_mock: bool; is_prod: bool |}
                                         gcs: {| is_mock: bool |}
                                         ml:
                                             {| is_mock: bool
                                                time_out: System.TimeSpan |} |}>

    resp.StatusCode |> should equal HttpStatusCode.OK

    body.db |> should equal {| is_mock = true; is_prod = false |}

    body.ml.is_mock |> should equal true
    body.gcs.is_mock |> should equal true


[<Fact>]
let healthML () =
    let resp =
        client.GetAsync("/health/ml") |> Async.AwaitTask |> Async.RunSynchronously

    let body =
        resp.Content.ReadAsStringAsync().Result
        |> JsonSerializer.Deserialize<{| data: {| status: string; device: string |} |}>

    resp.StatusCode |> should equal HttpStatusCode.OK
    body.data.status |> should equal "ok"
