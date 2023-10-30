module Application.Infra

open Domain.Types

type InfraError =
    | Failure of string
    | Timeout of string

type MLHealthResp = { status: string; device: string }

type MLService = {
    health: unit -> Async<Result<MLHealthResp, InfraError>>
    inference: InferenceRequest -> Async<Result<InferenceResponse, InfraError>>
}
