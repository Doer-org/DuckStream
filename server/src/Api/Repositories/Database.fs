module Repository.Database

open FsToolkit.ErrorHandling

open System.Data
open Dapper
open Dapper.FSharp.PostgreSQL
open Npgsql
open Domain.Types
open Application.Persistence
open System

OptionTypes.register ()

type DBEnv = {
    IS_PROD: bool
    DB_HOST: string
    DB_PORT: int
    DB_USER: string
    DB_PASSWORD: string
    DB_NAME: string
}

let conn (env: DBEnv) : IDbConnection =
    let connStr =
        if env.IS_PROD then
            $"Host={env.DB_HOST};Port={env.DB_PORT};Database={env.DB_NAME};Username={env.DB_USER};Password={env.DB_PASSWORD};SslMode=Require;Trust Server Certificate=true"
        else
            $"Host={env.DB_HOST};Username={env.DB_USER};Password={env.DB_PASSWORD};Database={env.DB_NAME}"

    new NpgsqlConnection(connStr)

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

module Table =

    type gcs_image = {
        img_id: string
        img_url: string
        created_at: DateTime
    }

    type inference_result = {
        input_img_id: string
        result_img_id: string
        prompt: string
        converted_prompt: string
    }

module QueryResult =
    type GetInferenceResults = {
        input_img_id: string
        input_img_url: string
        result_img_id: string
        result_img_url: string
        prompt: string
        converted_prompt: string
    }


let duckstreamRepo env : Application.Persistence.DuckStreamImageRepo = {
    saveImage =
        fun morphoto ->
            let conn = conn env

            insert {
                into table<Table.gcs_image>

                value {
                    img_id = morphoto.id
                    img_url = morphoto.url
                    created_at = DateTime.UtcNow
                }
            }
            |> conn.InsertAsync
            |> Task.map (fun _ -> morphoto)
            |> AsyncResult.ofTask
            |> AsyncResult.mapError (fun e -> DB e.Message)
    getImage =
        fun img_id ->
            let conn = conn env

            select {
                for m in table<Table.gcs_image> do
                    where (m.img_id = img_id)
                    take 1
            }
            |> conn.SelectAsync<Table.gcs_image>
            |> Task.map (
                Seq.tryHead
                >> function
                    | Some s -> Ok { id = s.img_id; url = s.img_url }
                    | None -> Error(DB "not found")
            )
            |> AsyncResult.ofTask
            |> AsyncResult.mapError (fun e -> DB e.Message)
            |> Async.map (Result.bind id)

    getInferenceResults =
        fun img_id ->
            let conn = conn env

            let sql =
                """
                SELECT 
                    ir.input_img_id AS input_img_id,
                    i1.img_url AS input_img_url,
                    ir.result_img_id AS result_img_id,
                    i2.img_url AS result_img_url,
                    ir.prompt AS prompt,
                    ir.converted_prompt AS converted_prompt
                FROM inference_result ir
                INNER JOIN gcs_image i1 ON i1.img_id = ir.input_img_id
                INNER JOIN gcs_image i2 ON i2.img_id = ir.result_img_id
                WHERE ir.input_img_id = @img_id;
                """

            conn.QueryAsync<QueryResult.GetInferenceResults>(sql, {| img_id = img_id |})
            |> Task.map (
                Seq.map (fun row -> {
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
                })
                >> Seq.toArray
                >> Ok
            )
            |> AsyncResult.ofTask
            |> AsyncResult.mapError (fun e -> DB e.Message)
            |> Async.map (Result.bind id)

    registerInferenceResult =
        fun result ->
            let conn = conn env

            insert {
                into table<Table.inference_result>

                value {
                    input_img_id = result.input_image.id
                    result_img_id = result.result_image.id
                    prompt = result.prompt
                    converted_prompt = result.converted_prompt
                }
            }
            |> conn.InsertAsync
            |> Task.map (fun _ -> ())
            |> AsyncResult.ofTask
            |> AsyncResult.mapError (fun e -> DB e.Message)
}
