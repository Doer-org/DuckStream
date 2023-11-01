module Repository.Mock

open System

let mockGCS: Application.Persistence.GCStorageRepo = {
    upload =
        fun base64 ->
            async {
                let resp: Domain.Types.Image = {
                    id = Guid.NewGuid().ToString()
                    url = "img_url"
                }

                return resp |> Ok
            }
    downloadBase64 =
        fun id ->
            async {
                let resp: Domain.Types.Base64 = "base64"
                return resp |> Ok
            }
}


let mockMLService: Application.Infra.MLService = {
    inference =
        fun request ->
            async {
                let resp: Application.Infra.MLResult = {
                    image_base64 = request.image_base64
                    prompt = request.prompt
                    converted_prompt = "converted_prompt"
                }

                return resp |> Ok
            }
    health =
        fun () ->
            async {
                let resp: Application.Infra.MLHealthResp = {
                    status = "ok"
                    device = "device"
                }

                return resp |> Ok
            }
}
