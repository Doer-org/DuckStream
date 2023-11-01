module Application.Persistence

open Domain.Types

type PersistenceError =
    | DB of string
    | ImageNotFound of Id
    | InferenceFailed of Id
    | GCS of string

type DuckStreamImageRepo = {
    saveImage: Image -> Async<Result<Image, PersistenceError>>
    getImage: Id -> Async<Result<Image, PersistenceError>>
    getInferenceResult: Id -> Async<Result<InferenceResult, PersistenceError>>
    registerInferenceResult: InferenceResult -> Async<Result<unit, PersistenceError>>
}


type GCStorageRepo = {
    upload: Base64 -> Async<Result<Image, PersistenceError>>
    downloadBase64: Id -> Async<Result<Base64, PersistenceError>>
}
