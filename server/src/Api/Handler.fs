module Api.Handler

open Falco
open Application.CommandHandler
open System.Text.Json
open FsToolkit.ErrorHandling
open Domain.Types

let errorHandler (isDev: bool) (e: CommandError) : HttpHandler =
    if isDev then
        Response.withStatusCode 500
        >> Response.ofJson {| error = sprintf "%A" e |}
    else
        Response.withStatusCode 500
        >> Response.ofJson {| error = "error!" |}

let saveImage
    (errorHandler: CommandError -> HttpHandler)
    (repos: Repositories)
    : HttpHandler =
    fun ctx ->
        task {
            let! request =
                Request.getJsonOptions<{| base64: string |}>
                    JsonSerializerOptions.Default
                    ctx

            let ret = saveImage repos request.base64 |> Async.RunSynchronously

            return
                match ret with
                | Ok image -> Response.ofJson {| data = image |} ctx
                | Error e -> errorHandler e ctx
        }

let getImage
    (errorHandler: CommandError -> HttpHandler)
    (repos: Repositories)
    : HttpHandler =
    fun ctx ->
        task {
            let route = Request.getRoute ctx
            let id: Id = route.Get("id")

            let ret = getImage repos id |> Async.RunSynchronously

            return
                match ret with
                | Ok image -> Response.ofJson {| data = image |} ctx
                | Error e -> errorHandler e ctx
        }

let healthML
    (errorHandler: CommandError -> HttpHandler)
    (repos: Repositories)
    : HttpHandler =
    fun ctx ->
        task {
            let ret = healthML repos |> Async.RunSynchronously

            return
                match ret with
                | Ok resp -> Response.ofJson {| data = resp |} ctx
                | Error e -> errorHandler e ctx
        }

let inference
    (errorHandler: CommandError -> HttpHandler)
    (repos: Repositories)
    : HttpHandler =
    fun ctx ->
        task {
            let! request =
                Request.getJsonOptions<InferenceRequest>
                    JsonSerializerOptions.Default
                    ctx

            let ret = inference repos request |> Async.RunSynchronously

            return
                match ret with
                | Ok resp -> Response.ofJson {| data = resp |} ctx
                | Error e -> errorHandler e ctx
        }
