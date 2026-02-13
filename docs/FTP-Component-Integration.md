# FTP Component Integration (Razor Pages)

Tato komponenta se integruje jako partial do stránky (typicky Admin).

## 1. Inicializace v PageModel

```csharp
public FTP.FTP_UpdateModel fTP_Update;

public AdminModel(IConfiguration con)
{
    fTP_Update = new FTP.FTP_UpdateModel(con);
}

public async Task OnGetAsync()
{
    await fTP_Update.OnGetAsync();
}
```

## 2. Endpointy (handlery)

Do stejného `PageModel` vlož následující handlery:

```csharp
#region FTP

public async Task<IActionResult> OnPostFtpDeployAsync([FromForm] FTP.FTP_UpdateModel.FtpDeployRequest request)
{
    var result = await fTP_Update.OnPostFtpDeployAsync(request);
    return result;
}

public async Task<IActionResult> OnPostFtpReadConfigAsync([FromForm] FTP.FTP_UpdateModel.FtpConfigRequest request)
{
    var result = await fTP_Update.OnPostFtpReadConfigAsync(request);
    return result;
}

public async Task<IActionResult> OnPostFtpWriteConfigAsync([FromForm] FTP.FTP_UpdateModel.FtpConfigRequest request)
{
    var result = await fTP_Update.OnPostFtpWriteConfigAsync(request);
    return result;
}

public async Task<IActionResult> OnPostFtpToggleOfflineAsync([FromForm] FTP.FTP_UpdateModel.FtpToggleRequest request)
{
    var result = await fTP_Update.OnPostFtpToggleOfflineAsync(request);
    return result;
}

public async Task<IActionResult> OnPostFtpDeleteWebAsync([FromForm] FTP.FTP_UpdateModel.FtpDeleteRequest request)
{
    var result = await fTP_Update.OnPostFtpDeleteWebAsync(request);
    return result;
}

public async Task<IActionResult> OnPostFtpUploadFullAsync([FromForm] FTP.FTP_UpdateModel.FtpUploadRequest request)
{
    var result = await fTP_Update.OnPostFtpUploadFullAsync(request);
    return result;
}

#endregion
```

## 3. Vložení partialu do stránky

Do `.cshtml` stránky:

```cshtml
@await Html.PartialAsync("~/Areas/FTP/FTP_Update.cshtml", Model.fTP_Update)
```

## 4. Otevření modalu

Na stránce přidej tlačítko:

```html
<button class="btn btn-outline-secondary" type="button" data-bs-toggle="modal" data-bs-target="#ftpDeployModal">
    Otevřít správu
</button>
```

## 5. Další vazby

- Komponenta nepoužívá samostatné JS soubory, skripty jsou inline v `FTP_Update.cshtml`.
- Potřebuje Bootstrap modal (`bootstrap.bundle.js`).
- Partial obsahuje `@Html.AntiForgeryToken()` a fetch požadavky token posílají ve `FormData`.
- `FTP_UpdateModel` má `[Authorize]`, proto stránka/uživatel musí být autorizovaný.
