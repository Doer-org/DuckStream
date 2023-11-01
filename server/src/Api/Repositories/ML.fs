module Repository.ML

open System
open System.Net
open FsHttp
open Application.Infra

type MLEnv = { ML_URL: string; TIMEOUT: TimeSpan }

let inline private inference (request: MLInput) (env: MLEnv) =
    http {
        POST $"{env.ML_URL}/inference"
        body
        jsonSerialize request
        config_timeout env.TIMEOUT
    }
    |> Request.sendAsync
    |> Async.map (fun resp ->
        if resp.statusCode = HttpStatusCode.OK then
            resp |> Response.deserializeJson<MLResult> |> Ok
        else
            Error(Timeout "ML Server Timeout Error"))

let inline private health (env: MLEnv) =
    http {
        GET $"{env.ML_URL}/health"
        body
    }
    |> Request.sendAsync
    |> Async.map (fun resp ->
        if resp.statusCode = HttpStatusCode.OK then
            resp |> Response.deserializeJson<MLHealthResp> |> Ok
        else
            Error(Failure "ML Server Error"))


let mlRepo env : Application.Infra.MLService = {
    inference = fun request -> inference request env
    health = fun () -> health env
}
