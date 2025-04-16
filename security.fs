namespace ProtectEmail

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open System.Text.RegularExpressions
open System.Linq.Expressions

let protectedDomains = [ "example.com"; "sample.net"; "gmail.com" ]

type EmailRequest = { Email: string }

let isValidEmail email =
    Regex.IsMatch(email, @"[^@]+@[^@]+\.[^@]+")

let protectEmailHandler (context: HttpContext) =
    async {
        match context.Request.Method with
        | "POST" ->
            try
                let! body = context.Request.ReadFromJsonAsync<EmailRequest>()
                match body with
                | Some req ->
                    let email = req.Email.ToLower()
                    if not (isValidEmail email) then
                        context.Response.StatusCode <- 400
                        do! context.Response.WriteAsJsonAsync( {| error = "無効なメールアドレス形式です" |} )
                    else
                        let parts = email.Split '@'
                        if parts.Length <> 2 then
                            context.Response.StatusCode <- 400
                            do! context.Response.WriteAsJsonAsync( {| error = "無効なメールアドレス形式です" |} )
                        else
                            let username = parts.[0]
                            let domain = parts.[1]
                            if protectedDomains |> List.contains domain then
                                let maskedUsername = username.Substring(0, 2) + "***"
                                let maskedEmail = $"{maskedUsername}@{domain}"
                                do! context.Response.WriteAsJsonAsync( {| protected_email = maskedEmail |} )
                            else
                                do! context.Response.WriteAsJsonAsync( {| message = "このドメインのメールアドレスは保護対象外です" |} )
                | None ->
                    context.Response.StatusCode <- 400
                    do! context.Response.WriteAsJsonAsync( {| error = "メールアドレスが送信されていません" |} )
            with _ ->
                context.Response.StatusCode <- 500
                do! context.Response.WriteAsJsonAsync( {| error = "サーバーエラー" |} )
        | _ ->
            context.Response.StatusCode <- 405
            do! context.Response.WriteAsJsonAsync( {| error = "POSTリクエストのみ受け付けます" |} )
    }

[<EntryPoint>]
let main _ =
    let builder = WebApplication.CreateBuilder()
    let app = builder.Build()

    app.MapPost("/protect", protectEmailHandler)

    app.Run()
    0
