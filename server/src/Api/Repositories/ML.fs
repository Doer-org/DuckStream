module Repository.ML

open System
open System.Net
open FsHttp
open Application.Infra
open Domain.Types

type MLEnv = { ML_URL: string; TIMEOUT: TimeSpan }

let inference (request: InferenceRequest) (env: MLEnv) =
    http {
        POST $"{env.ML_URL}/inference"
        body
        jsonSerialize request
        config_timeout env.TIMEOUT
    }
    |> Request.sendAsync
    |> Async.map (fun resp ->
        if resp.statusCode = HttpStatusCode.OK then
            resp |> Response.deserializeJson<InferenceResponse> |> Ok
        else
            Error(Timeout "ML Server Timeout Error"))

let mlRepo env : Application.Infra.MLService = {
    inference = fun request -> inference request env
    health =
        fun () ->
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
}
