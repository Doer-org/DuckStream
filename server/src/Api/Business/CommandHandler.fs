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
    imageInfo: ImageInfoRepo
    rawImage: RawImageRepo
    ml: MLService
}

let getImage (repos: Repositories) (id: Id) =
    asyncResult {
        let! image =
            repos.imageInfo.retrieve id
            |> AsyncResult.mapError Persistence

        // return ImageRetrieved image
        return image
    }

let saveImage (repos: Repositories) (base64: Base64) =
    asyncResult {
        let! obj =
            repos.rawImage.upload base64
            |> AsyncResult.mapError Persistence

        let! image =
            repos.imageInfo.register {
                id = obj.Id
                base64 = base64
                url = obj.MediaLink
            }
            |> AsyncResult.mapError Persistence


        return ImageSaved image
    }

let inference (repos: Repositories) (request: InferenceRequest) =
    asyncResult {
        let! result = repos.ml.inference request |> AsyncResult.mapError Infra
        return InferenceCompleted result
    }

// let commandHandler (repos: Repositories) (command: Command) =
//     match command with
//     | GetImage id -> getImage repos.imageInfo id
//     | SaveImage base64 -> saveImage repos.imageInfo repos.rawImage base64
//     | Inference request -> inference repos.ml request
