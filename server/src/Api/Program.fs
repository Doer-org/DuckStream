open Falco
open Falco.Routing
open Falco.HostBuilder
open Application.CommandHandler

open System
open DotNetEnv

type Environment =
    | Local
    | Mock
    | Prod

type AppEnv = {
    environment: Environment
    client_url: string
    db: Repository.Database.DBEnv
    gcs: Repository.GCS.GCS_ENV
    ml: Repository.ML.MLEnv
}

let env =
    let isLocal =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") = "Development"

    if isLocal then
        Env.Load("../../.env") |> ignore

    let ENVIRONMENT =
        match Env.GetString("ENVIRONMENT") with
        | "local" -> Local
        | "mock" -> Mock
        | _ -> Prod

    {
        environment = ENVIRONMENT
        db = {
            IS_DEV = ENVIRONMENT <> Prod
            DB_HOST = Env.GetString("DB_HOST")
            DB_PORT = Env.GetInt("DB_PORT")
            DB_USER = Env.GetString("DB_USER")
            DB_PASSWORD = Env.GetString("DB_PASSWORD")
            DB_NAME = Env.GetString("DB_NAME")
        }
        gcs = {
            GCS_CREDENTIALS = Env.GetString("GCS_CREDENTIALS")
            GCS_BUCKET_NAME = Env.GetString("GCS_BUCKET_NAME")
            GCS_URL = Env.GetString("GCS_URL")
        }
        ml = {
            ML_URL = Env.GetString("ML_URL")
            TIMEOUT = TimeSpan.FromSeconds(Env.GetDouble("ML_REQUEST_TIMEOUT_SEC"))
        }
        client_url = Env.GetString("CLIENT_URL")
    }

type Program =
    class
    end


[<EntryPoint>]
let main _ =
    let errorHandler = Api.Handler.errorHandler (env.environment <> Prod)

    let repos =
        if env.environment = Mock then
            {
                duckstreamImage = Repository.Database.duckstreamRepo env.db
                GCStorage = Repository.Mock.mockGCS
                mlService = Repository.Mock.mockMLService
            }
        else
            {
                duckstreamImage = Repository.Database.duckstreamRepo env.db
                GCStorage = Repository.GCS.gcsStore env.gcs
                mlService = Repository.ML.mlRepo env.ml
            }

    webHost [||] {
        endpoints [
            get "/" (Response.ofJson {| hello = "world" |})
            get "/health" (Response.ofJson {| env = env |})
            get "/health/ml" (Api.Handler.healthML errorHandler repos)
            post "/image" (Api.Handler.saveImage errorHandler repos)
            get "/image/{id}" (Api.Handler.getImage errorHandler repos)
            post "/inference" (Api.Handler.inference errorHandler repos)
        ]
    }

    0
