module Repository.Mock

open System

let mockGCS: Application.Persistence.GCStorageRepo = {
    upload =
        fun _ ->
            async {
                let resp: Domain.Types.Image = {
                    id = Guid.NewGuid().ToString()
                    url = "img_url"
                }

                return resp |> Ok
            }
    downloadBase64 =
        fun _ ->
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
                    converted_image = request.image
                    prompt = request.prompt
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

open Database.Table
open Application.Persistence
open Domain.Types

let mockDB: Application.Persistence.DuckStreamImageRepo =
    let mutable images: Database.Table.gcs_image list = []
    let mutable inference_result: Database.Table.inference_result list = []

    {
        saveImage =
            fun _ ->
                async {
                    let new_row = {
                        img_id = Guid.NewGuid().ToString()
                        img_url = "img_url"
                        created_at = DateTime.Now
                    }

                    images <- new_row :: images

                    let resp: Domain.Types.Image = {
                        id = new_row.img_id
                        url = new_row.img_url
                    }


                    return resp |> Ok
                }
        getImage =
            fun id ->
                async {
                    let resp =
                        images
                        |> List.tryFind (fun row -> row.img_id = id)
                        |> function
                            | Some row ->
                                let resp: Domain.Types.Image = {
                                    id = row.img_id
                                    url = row.img_url
                                }

                                resp |> Ok
                            | None -> Error(PersistenceError.DB "not found")

                    return resp
                }
        getInferenceResults =
            fun id ->
                async {
                    let resp =
                        query {
                            for ir in inference_result do
                                join i1 in images on (ir.input_img_id = i1.img_id)
                                join i2 in images on (ir.result_img_id = i2.img_id)
                                where (ir.input_img_id = id)

                                select {|
                                    input_img_id = ir.input_img_id
                                    input_img_url = i1.img_url
                                    result_img_id = ir.result_img_id
                                    result_img_url = i2.img_url
                                    prompt = ir.prompt
                                    converted_prompt = ir.converted_prompt
                                |}
                        }

                        |> Seq.map (fun row ->

                            let r: InferenceResult = {
                                input_image = {
                                    id = row.input_img_id
                                    url = row.input_img_url
                                }
                                result_image = {
                                    id = row.result_img_id
                                    url = row.result_img_url
                                }
                                prompt = row.prompt
                                converted_prompt = row.converted_prompt
                            }

                            r)
                        |> Seq.toArray
                        |> Ok

                    return resp
                }
        registerInferenceResult =
            fun result ->
                async {
                    inference_result <-
                        {
                            input_img_id = result.input_image.id
                            result_img_id = result.result_image.id
                            prompt = result.prompt
                            converted_prompt = result.converted_prompt
                        }
                        :: inference_result

                    return Ok()
                }
    }
