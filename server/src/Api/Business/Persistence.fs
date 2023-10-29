module Application.Persistence

open Domain.Types

type PersistenceError =
    | DB of string
    | ImageNotFound of Id
    | InferenceFailed of Id
    | GCS of string

type ImageInfoRepo = {
    register: Image -> Async<Result<Image, PersistenceError>>
    retrieve: Id -> Async<Result<Image, PersistenceError>>
}

type RawImageRepo = {
    upload:
        Base64 -> Async<Result<Google.Apis.Storage.v1.Data.Object, PersistenceError>>
    downloadBase64: Id -> Async<Result<Base64, PersistenceError>>
}
