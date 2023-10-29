open Falco
open Falco.Routing
open Falco.HostBuilder

[<EntryPoint>]
let main args =
    webHost [||] {
        endpoints [
            get "/health" (Response.ofJson {| env = "local" |})
            get "/health/ml" (Response.ofJson {| env = "local" |})

            get "/image/{id}" (Response.ofJson {| id = "1"; img = "base64" |})

            (*
                input: { parent_image: base64 }
            *)
            post
                "/inference"
                (Response.ofJson {|
                    parent_id = "1"
                    child_id = "1"
                    img = "base64"
                |})

        ]
    }

    0 // Exit code
