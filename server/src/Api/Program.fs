open Falco
open Falco.Routing
open Falco.HostBuilder
open Application.CommandHandler

open System
open DotNetEnv

type Environment =
    | Local
    | Prod

type AppEnv = {
    environment: Environment
    client_url: string
    db: Repository.Database.DBEnv
    gcs: Repository.GCS.GCS_ENV
    ml: Repository.ML.MLEnv
    is_mock: {| ml: bool; db: bool; gcs: bool |}
}

let env =
    let isLocal =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") = "Development"

    if isLocal then
        Env.Load("../../.env") |> ignore

    let ENVIRONMENT =
        match Env.GetString("ENVIRONMENT") with
        | "production" -> Prod
        | _ -> Local

    let is_mock = {|
        ml = Env.GetBool("MOCK_ML")
        db = Env.GetBool("MOCK_DB")
        gcs = Env.GetBool("MOCK_GCS")
    |}

    {
        environment = ENVIRONMENT
        db = {
            IS_PROD = (not is_mock.db) && (ENVIRONMENT = Prod)
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
        is_mock = is_mock
    }

type Program =
    class
    end


[<EntryPoint>]
let main _ =
    let errorHandler = Api.Handler.errorHandler (env.environment <> Prod)

    let repos = {
        duckstreamImage =
            if env.is_mock.db then
                Repository.Mock.mockDB
            else
                Repository.Database.duckstreamRepo env.db

        GCStorage =
            if env.is_mock.db then
                Repository.Mock.mockGCS
            else
                Repository.GCS.gcsStore env.gcs


        mlService =
            if env.is_mock.ml then
                Repository.Mock.mockMLService
            else
                Repository.ML.mlRepo env.ml
    }


    let healthHandler =
        let envInfo = {|
            db = {|
                is_mock = env.is_mock.db
                is_prod = env.db.IS_PROD
            |}
            gcs = {| is_mock = env.is_mock.gcs |}
            ml = {|
                is_mock = env.is_mock.ml
                time_out = env.ml.TIMEOUT
            |}
            client_url = env.client_url
        |}

        Response.ofJson envInfo

    webHost [||] {
        use_cors "CorsPolicy" (fun options ->

            options.AddPolicy(
                "CorsPolicy",
                fun policyBuilder ->
                    policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                    //
                    // https://duckstream-6am45n7bnq-an.a.run.app
                    // .WithOrigins(
                    //     [|
                    //         env.client_url
                    //         "http://localhost:3000"
                    //         "https://duck-stream.doer-app.com/"
                    //     |]
                    // )
                    |> ignore
            ))

        endpoints [
            get "/" (Response.ofJson {| hello = "world" |})
            get "/health" healthHandler
            get "/health/ml" (Api.Handler.healthML errorHandler repos)
            post "/image" (Api.Handler.saveImage errorHandler repos)
            get "/image/{id}" (Api.Handler.getImage errorHandler repos)
            post "/inference" (Api.Handler.inference errorHandler repos)
            get "/inference/{id}" (Api.Handler.getResults errorHandler repos)
        ]
    }

    0
