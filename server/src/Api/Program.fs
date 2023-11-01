open Falco
open Falco.Routing
open Falco.HostBuilder
open Application.CommandHandler

open System

type Env = {
    environment: string
    client_url: string
    db: Repository.Database.DBEnv
    gcs: Repository.GCS.GCS_ENV
    ml: Repository.ML.MLEnv
}

let env =
    let isLocal =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") = "Development"

    if isLocal then
        // dotnetコマンドから起動するとき
        DotNetEnv.Env.Load("../.env") |> ignore

    let ENVIRONMENT = Environment.GetEnvironmentVariable("ENVIRONMENT")

    {
        environment = ENVIRONMENT
        db = {
            IS_DEV = ENVIRONMENT = "Development"
            DB_HOST = Environment.GetEnvironmentVariable("DB_HOST")
            DB_USER = Environment.GetEnvironmentVariable("DB_USER")
            DB_PASSWORD = Environment.GetEnvironmentVariable("DB_PASSWORD")
            DB_NAME = Environment.GetEnvironmentVariable("DB_NAME")
        }
        gcs = {
            GCS_CREDENTIALS = Environment.GetEnvironmentVariable("GCS_CREDENTIALS")
            GCS_BUCKET_NAME = Environment.GetEnvironmentVariable("GCS_BUCKET_NAME")
            GCS_URL = Environment.GetEnvironmentVariable("GCS_URL")
        }
        ml = {
            ML_URL = Environment.GetEnvironmentVariable("ML_URL")
            TIMEOUT =
                TimeSpan.FromSeconds(
                    // Environment.GetEnvironmentVariable("ML_REQUEST_TIMEOUT_SEC")
                    // |> Double.Parse
                    30.0
                )
        }
        client_url = "aaa" // Environment.GetEnvironmentVariable("CLIENT_URL")
    }

type Program =
    class
    end


[<EntryPoint>]
let main args =
    let isTest = args.Length > 0 && args[0] = "test"

    printfn "isTest: %b" isTest

    let env = {
        env with
            environment = if isTest then "test" else env.environment
    }


    let errorHandler = Api.Handler.errorHandler true

    let repos = {
        duckstreamImage = Repository.Database.duckstreamRepo env.db
        GCStorage = Repository.GCS.gcsStore env.gcs
        mlService = Repository.ML.mlRepo env.ml
    }

    webHost [||] {
        endpoints [
            get "/" (Response.ofJson {| hello = "world" |})
            get "/health" (Response.ofJson {| env = env.environment |})
            get "/health/ml" (Api.Handler.healthML errorHandler repos)
            post "/image" (Api.Handler.saveImage errorHandler repos)
            get "/image/{id}" (Api.Handler.getImage errorHandler repos)
            post "/inference/{id}" (Api.Handler.inference errorHandler repos)
        ]
    }

    0
