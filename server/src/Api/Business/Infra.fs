module Application.Infra

open Domain.Types

type InfraError =
    | Failure of string
    | Timeout of string

type MLHealthResp = { status: string; device: string }

type MLInput = {
    prompt: string
    image: Base64
    strength: float
}

type MLResult = {
    converted_image: Base64
    prompt: string
}

type MLService = {
    health: unit -> Async<Result<MLHealthResp, InfraError>>
    inference: MLInput -> Async<Result<MLResult, InfraError>>
}
