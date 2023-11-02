module Tests

open Xunit
open FsHttp
open System.Net
open FsUnit.Xunit
open Microsoft.AspNetCore.Mvc.Testing

open System.Text.Json
open System.Net.Http
open System.Text

System.Environment.SetEnvironmentVariable("MOCK_DB", "true")
System.Environment.SetEnvironmentVariable("MOCK_ML", "true")
System.Environment.SetEnvironmentVariable("MOCK_GCS", "true")

let client =
    let webapp = new WebApplicationFactory<Program.Program>()
    webapp.CreateClient()


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

let save_random_image<'Response> () =
    client.PostAsync(
        "/image",
        new StringContent(
            JsonSerializer.Serialize<{| base64: string |}>({| base64 = "base64" |}),
            Encoding.UTF8,
            "application/json"
        )
    )
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> fun resp ->
        let body =
            resp.Content.ReadAsStringAsync().Result |> JsonSerializer.Deserialize<'Response>

        resp.StatusCode |> should equal HttpStatusCode.OK
        body


[<Fact>]
let save_and_get_image () =
    let body = save_random_image<{| data: {| id: string; url: string |} |}> ()

    let resp =
        client.GetAsync($"/image/{body.data.id}")
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let body =
        resp.Content.ReadAsStringAsync().Result
        |> JsonSerializer.Deserialize<{| data: {| id: string; url: string |} |}>

    resp.StatusCode |> should equal HttpStatusCode.OK


[<Fact>]
let inference () =
    let input_image = save_random_image<{| data: {| id: string; url: string |} |}> ()

    let inference () =
        client.PostAsync(
            "/inference",
            new StringContent(
                JsonSerializer.Serialize<{| id: string; prompt: string |}>(
                    {| id = input_image.data.id
                       prompt = "prompt" |}
                ),
                Encoding.UTF8,
                "application/json"
            )
        )
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let result_images =
        [| for _ in 1..3 ->
               inference ()
               |> fun ret ->
                   let body =
                       ret.Content.ReadAsStringAsync().Result
                       |> JsonSerializer.Deserialize<{| data: {| id: string; url: string |} |}>

                   body.data |]

    let resp =
        client.GetAsync($"/inference/{input_image.data.id}")
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let body =
        resp.Content.ReadAsStringAsync().Result
        |> JsonSerializer.Deserialize<{| data: Domain.Types.InferenceResult[] |}>

    resp.StatusCode |> should equal HttpStatusCode.OK
    body.data.Length |> should equal result_images.Length
