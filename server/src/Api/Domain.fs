module Domain

module Types =

    type Id = string

    type Base64 = string

    type Image = { id: Id; base64: Base64; url: string }

    type InferenceRequest = { prompt: string; image_base64: Base64 }

    type InferenceResponse = { id: Id; image: Image }

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
        | InferenceCompleted of InferenceResponse
