module Application.Infra

open Domain.Types

type InfraError =
    | Failure of string
    | Timeout of string

type MLHealthResp = { status: string; device: string }

type MLInput = { prompt: string; image_base64: Base64 }

type MLResult = {
    image_base64: Base64
    prompt: string
    converted_prompt: string
}

type MLService = {
    health: unit -> Async<Result<MLHealthResp, InfraError>>
    inference: MLInput -> Async<Result<MLResult, InfraError>>
}
