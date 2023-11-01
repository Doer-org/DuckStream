module Domain

module Types =

    type Id = string

    type Base64 = string

    type Image = { id: Id; url: string }

    type InferenceRequest = { prompt: string; id: Id }

    type InferenceResult = {
        input_image: Image
        result_image: Image
        prompt: string
        converted_prompt: string
    }

    type DomainError =
        /// 既に推論済みの画像を再度推論したら
        | AlreadyInferred of Id

    type Command =
        | GetImage of Id
        | SaveImage of Base64
        | Inference of InferenceRequest

    type Event =
        | ImageRetrieved of Image
        | ImageSaved of Image
        | InferenceCompleted of InferenceResult
