module Application.CommandHandler

open Domain.Types
open Application.Persistence
open Application.Infra
open FsToolkit.ErrorHandling

type CommandError =
    | Domain of DomainError
    | Persistence of PersistenceError
    | Infra of InfraError

type Repositories = {
    duckstreamImage: DuckStreamImageRepo
    GCStorage: GCStorageRepo
    mlService: MLService
}

let getImage (repos: Repositories) (id: Id) =
    asyncResult {
        let! image =
            repos.duckstreamImage.retrieve id
            |> AsyncResult.mapError Persistence

        return image
    }

let saveImage (repos: Repositories) (base64: Base64) =
    asyncResult {
        let! obj =
            repos.GCStorage.upload base64
            |> AsyncResult.mapError Persistence

        let! image =
            repos.duckstreamImage.register {
                id = obj.Id
                base64 = base64
                url = obj.MediaLink
            }
            |> AsyncResult.mapError Persistence

        return image
    }

let healthML (repos: Repositories) =
    asyncResult {
        let! result = repos.mlService.health () |> AsyncResult.mapError Infra
        return result
    }

let inference (repos: Repositories) (request: InferenceRequest) =
    asyncResult {
        let! result =
            repos.mlService.inference request
            |> AsyncResult.mapError Infra

        return result
    }
