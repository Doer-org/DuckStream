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
            repos.duckstreamImage.getImage id
            |> AsyncResult.mapError Persistence

        return image
    }

let saveImage (repos: Repositories) (base64: Base64) =
    asyncResult {
        let! obj =
            repos.GCStorage.upload base64
            |> AsyncResult.mapError Persistence

        let! image =
            repos.duckstreamImage.saveImage { id = obj.id; url = obj.url }
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
        let! base64 =
            repos.GCStorage.downloadBase64 request.id
            |> AsyncResult.mapError Persistence

        let! ml_result =
            repos.mlService.inference {
                prompt = request.prompt
                image_base64 = base64
            }
            |> AsyncResult.mapError Infra

        let! result_image = saveImage repos ml_result.image_base64

        let! input_image = getImage repos request.id

        let! _ =
            let result = {
                input_image = input_image
                result_image = result_image
                prompt = request.prompt
                converted_prompt = ml_result.converted_prompt
            }

            repos.duckstreamImage.registerInferenceResult result
            |> AsyncResult.mapError Persistence

        return result_image
    }

let getInferenceResults id (repos: Repositories) =
    asyncResult {
        let! results =
            repos.duckstreamImage.getInferenceResults id
            |> AsyncResult.mapError Persistence

        return results
    }
