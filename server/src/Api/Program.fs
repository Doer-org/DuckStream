open Falco
open Falco.Routing
open Falco.HostBuilder
open Application.CommandHandler

[<EntryPoint>]
let main _ =
    let errorHandler = Api.Handler.errorHandler true

    let repos = {
        duckstreamImage =
            Repository.Database.duckstreamRepo {
                IS_DEV = true
                DB_HOST = " "
                DB_USER = " "
                DB_PASSWORD = " "
                DB_DATABASE = " "
            }
        GCStorage =
            Repository.GCS.gcsStore {
                GCP_CREDENTIALS = " "
                GCP_BUCKET_NAME = " "
                GCS_URL = " "
            }
        mlService =
            Repository.ML.mlRepo {
                ML_URL = " "
                timeout = System.TimeSpan.FromSeconds(30.0)
            }
    }

    webHost [||] {
        endpoints [
            get "/health" (Response.ofJson {| env = " " |})
            get "/health/ml" (Api.Handler.healthML errorHandler repos)
            post "/image" (Api.Handler.saveImage errorHandler repos)
            get "/image/{id}" (Api.Handler.getImage errorHandler repos)
            post "/inference/{id}" (Api.Handler.inference errorHandler repos)
        ]
    }

    0
