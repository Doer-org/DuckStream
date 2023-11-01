module Repository.Database

open FsToolkit.ErrorHandling

open System.Data
open Dapper.FSharp.MySQL
open MySql.Data.MySqlClient
open Domain.Types
open Application.Persistence

OptionTypes.register ()

type DBEnv = {
    IS_DEV: bool
    DB_HOST: string
    DB_USER: string
    DB_PASSWORD: string
    DB_NAME: string
}

let conn (env: DBEnv) : IDbConnection =
    let connStr =
        if env.IS_DEV then
            $"Server={env.DB_HOST};Port=3306;Database={env.DB_NAME};user={env.DB_USER};password={env.DB_PASSWORD}"
        else
            $"Server={env.DB_HOST};Port=3306;Database={env.DB_NAME};user={env.DB_USER};password={env.DB_PASSWORD};SslMode=VerifyFull"

    new MySqlConnection(connStr)

(*
    複雑なクエリはこのように叩く
    let sql = SELECT * FROM Morphoto WHERE morphoto_id = @morphoto_id    
    let conn = conn env

    conn.QueryAsync<Morphoto>(
        sql,
        {| morphoto_id = morphoto_id |}
    )
    |> Task.map (Seq.toArray >> Ok) 
*)


let duckstreamRepo env : Application.Persistence.DuckStreamImageRepo = {
    register =
        fun morphoto ->
            let conn = conn env

            insert {
                into table<Image>
                value morphoto
            }
            |> conn.InsertAsync
            |> Task.map (fun _ -> morphoto)
            |> AsyncResult.ofTask
            |> AsyncResult.mapError (fun e -> DB e.Message)

    retrieve =
        fun parent_id ->
            let conn = conn env

            select {
                for m in table<Image> do
                    where (m.id = parent_id)
                    take 1
            }
            |> conn.SelectAsync<Image>
            |> Task.map (
                Seq.tryHead
                >> function
                    | Some s -> Ok s
                    | None -> Error(DB "not found")
            )
            |> AsyncResult.ofTask
            |> AsyncResult.mapError (fun e -> DB e.Message)
            |> Async.map (Result.bind id)
}
