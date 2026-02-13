# Web_FTP komponenta

Tento repozitář obsahuje pouze FTP komponentu pro Razor Pages.

## Obsah repozitáře

- `Areas/FTP/FTP_Update.cshtml`
- `Areas/FTP/FTP_Update.cshtml.cs`
- `docs/FTP-Component-Integration.md`
- `samples/Pages/Admin.cshtml`
- `samples/Pages/Admin.cshtml.cs`

## Co komponenta dělá

- FTP deploy release binárek (`dll/pdb/exe`)
- zapnutí/vypnutí `app_offline.htm`
- čtení/zápis `appsettings.json` a `web.config` přes FTP
- mazání vzdáleného webu (volitelně se zachováním `wwwroot`)
- full upload webu z lokální release složky

## Požadavky hostitelské aplikace

- ASP.NET Core Razor Pages
- stránka, která renderuje partial `~/Areas/FTP/FTP_Update.cshtml`
- endpoint handlery v `PageModel` (viz `docs/FTP-Component-Integration.md`)
- Bootstrap 5 (modal v komponentě)
- antiforgery (komponenta posílá `__RequestVerificationToken`)

## Konfigurace (host app)

Do `appsettings.json` host aplikace:

```json
{
  "Deploy": {
    "Visible": "True",
    "ProjectName": "Blog.Web",
    "LocalReleasePath": "D:\\Path\\To\\Release",
    "Ftp": {
      "Host": "ftp.example.com",
      "Username": "ftp-user",
      "Password": "ftp-password",
      "Port": "21",
      "RemoteRoot": "/",
      "UseSsl": "false"
    }
  }
}
```

Poznámka: v repozitáři nejsou žádné specifické externí JS soubory pro FTP komponentu. JavaScript je přímo uvnitř `FTP_Update.cshtml`.
