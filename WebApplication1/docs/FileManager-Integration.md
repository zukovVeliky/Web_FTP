# FileManager - Kompletní dokumentace nasazení a integrace

Tento dokument je psanı krok za krokem pro èlovìka, kterı zná základy ASP.NET Core/Razor, ale nechce sloitì dohledávat interní vazby projektu.

Dokumentace odpovídá aktuálnímu stavu projektu, kde je FileManager umístìn v:

- `wwwroot/lib/FileManager`
- `Areas/FileManeger`

Novı (hlavní) FileManager UI je stránka:

- `Pages/FileManager.cshtml`
- route: `/FileManager`

## 1. Co je kde

## 1.1 Novı FileManager (to, co máš pouívat)

- UI stránka: `Pages/FileManager.cshtml`
- PageModel: `Pages/FileManager.cshtml.cs`
- Frontend logika: `wwwroot/js/filemanager.js`
- Styly:
  - `wwwroot/css/windows-filemanager.css`
  - `wwwroot/css/filemanager.css`
  - `wwwroot/css/code-editor.css`
- Backend API:
  - `Areas/FileManeger/Controllers/FileManagerApiController.cs`
  - `Services/CustomFileManagerService.cs`

## 1.2 Legacy (starı) manager

Legacy èást je kvùli zpìtné kompatibilitì umístìna v:

- `Areas/FileManeger`
- `wwwroot/lib/FileManager`

Poznámka: název area je schválnì `FileManeger` (pøesnì takto), protoe takto je definováno zadání.

## 2. Minimální poadavky v Program.cs

Aby FileManager fungoval, musí bıt zapnuté Razor Pages, API controllery a statické soubory.

Zkontroluj, e v `Program.cs` máš:

```csharp
builder.Services.AddRazorPages();
builder.Services.AddScoped<WebApplication1.Services.CustomFileManagerService>();

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapControllers();
```

## 3. Spouštìní FileManageru

## 3.1 Spuštìní tlaèítkem (ve stejné záloce)

Pouij toto, kdy chceš pøejít na FileManager jako na klasickou stránku:

```html
<button type="button" onclick="window.location.href='/FileManager'">
  Otevøít FileManager
</button>
```

## 3.2 Spuštìní tlaèítkem v popup oknì

Pouij, kdy chceš FileManager otevøít oddìlenì a neodejít z aktuální stránky.

```html
<button type="button" onclick="openFileManagerWindow()">Otevøít FileManager</button>
<script>
function openFileManagerWindow() {
  window.open('/FileManager', 'FileManagerWindow', 'width=1280,height=820,resizable=yes,scrollbars=yes');
}
</script>
```

## 3.3 Spuštìní èistì skriptem

Napøíklad po kliknutí na vlastní UI prvek nebo po nìjaké události:

```js
window.open('/FileManager', 'FileManagerWindow', 'width=1280,height=820,resizable=yes,scrollbars=yes');
```

## 4. Parametry FileManageru

FileManager podporuje query parametry:

- `root` - koøen, ve kterém má manager zaèít (napø. `lib`, `UzivatelskeSoubory`)
- `picker` - reim vıbìru souboru (vrací URL do rodièovského okna)
- `allowExt` - whitelist pøípon
- `onlyImages` - omezí vıbìr na obrázky
- `callback` - název JS funkce v openeru

## 4.1 Formát parametrù (dùleité)

V projektu se pouívá Base64 URL-safe formát s prefixem `b64:`.

Pøíklad:

- text `lib`
- parametr: `b64:bGli`

Komponenta umí i legacy plain base64 (zpìtná kompatibilita), ale pro nové pouití dr vdy `b64:`.

## 5. Jak pøebírat data a URL z FileManageru na jiné stránce

## 5.1 Princip

1. Otevøeš `/FileManager` s `picker=1`.
2. Uivatel klikne na soubor.
3. FileManager zavolá ve `window.opener` callback funkci.
4. Callback dostane dva parametry:
   - `url` (URL vybraného souboru)
   - `fileName` (název souboru)

## 5.2 Praktickı pøíklad

Vlo do cílové stránky:

```html
<button type="button" onclick="openPicker()">Vybrat soubor</button>

<script>
function toBase64UrlUtf8(text) {
  const bytes = new TextEncoder().encode(String(text ?? ''));
  let binary = '';
  for (const b of bytes) binary += String.fromCharCode(b);
  return 'b64:' + btoa(binary).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/g, '');
}

function openPicker() {
  const root = toBase64UrlUtf8('UzivatelskeSoubory');
  const allowExt = toBase64UrlUtf8('jpg,jpeg,png,pdf,zip');
  const callback = toBase64UrlUtf8('OnFilePicked');
  const picker = toBase64UrlUtf8('1');

  const url = '/FileManager'
    + '?picker=' + encodeURIComponent(picker)
    + '&root=' + encodeURIComponent(root)
    + '&allowExt=' + encodeURIComponent(allowExt)
    + '&callback=' + encodeURIComponent(callback);

  window.open(url, 'FileManagerPicker', 'width=1280,height=820,resizable=yes,scrollbars=yes');
}

function OnFilePicked(url, fileName) {
  console.log('Vybranı soubor:', fileName, url);
  // Sem dej vlastní logiku (napø. vyplnit input, vloit URL do formuláøe, zavolat API apod.)
}
</script>
```

## 6. Root a fallback chování

- `root` vdy smìøuje do `wwwroot`.
- Pokud zadanı root neexistuje, komponenta automaticky fallbackne na default:
  - `UzivatelskeSoubory`

To znamená: i kdy pošleš neexistující `root`, aplikace nespadne.

## 7. Integrace do CKEditoru (tak, jak je provedena v projektu)

Aktuální implementace je v:

- `Areas/CKEditor5/Pages/CKEditor5.cshtml`

## 7.1 Oficiální dokumentace CKEditoru (doporuèené zdroje)

- Vytvoøení jednoduchého pluginu:
  - https://ckeditor.com/docs/ckeditor5/latest/framework/tutorials/creating-simple-plugin-timestamp.html
- Architektura UI komponent (vèetnì ikon):
  - https://ckeditor.com/docs/ckeditor5/latest/framework/architecture/ui-components.html#icons
- Konfigurace toolbaru:
  - https://ckeditor.com/docs/ckeditor5/latest/getting-started/setup/toolbar.html
- Základní pøehled pluginù:
  - https://ckeditor.com/docs/ckeditor5/latest/framework/architecture/plugins.html
- (Volitelné) Vlastní upload adapter:
  - https://ckeditor.com/docs/ckeditor5/latest/framework/deep-dive/upload-adapter.html

## 7.2 Co je v projektu implementováno

- vlastní toolbar plugin `FileManagerToolbarPlugin`
- dvì tlaèítka do toolbaru:
  - `fileManager`: otevøe picker pro obrázky + pdf
  - `fileManagerAll`: otevøe picker bez omezení typu souboru
- otevírání popupu na `/FileManager` s parametry:
  - `picker=1`
  - `allowExt` (u image/pfd varianty)
  - `callback=ZachyceniURLCKeditor`
  - `root` (podle `SetPath`)
- callback funkce `ZachyceniURLCKeditor(url, fileName)`:
  - pokud je to obrázek -> volá `insertImage`
  - jinak vloí text s `linkHref`

## 7.3 Krok za krokem implementace do jiné stránky s CKEditorem

1. Pøidej CKEditor skript na stránku.
2. Definuj SVG ikony tlaèítek (jako string).
3. Vytvoø plugin `FileManagerToolbarPlugin`.
4. V pluginu pøidej do `componentFactory` dvì tlaèítka (`fileManager`, `fileManagerAll`).
5. V obsluze `execute` otevøi popup na `/FileManager` a pošli parametry pøes `b64:`.
6. Do konfigurace editoru pøidej plugin do `extraPlugins`.
7. Pøidej `fileManager` a `fileManagerAll` do `toolbar.items`.
8. Definuj globální callback `ZachyceniURLCKeditor(url, fileName)`.
9. V callbacku zpracuj:
  - obrázek -> `insertImage`
  - jinı soubor -> text/odkaz
10. Otestuj:
  - otevøení popupu
  - vıbìr souboru
  - návrat URL do editoru

## 7.4 Kde máš v projektu zálohu této implementace

Kvùli opìtovnému pouití je záloha uloená v:

- `wwwroot/lib/FileManager/integration/ckeditor-filemanager-plugin.backup.js`
- `wwwroot/lib/FileManager/integration/ckeditor-filemanager-callback.backup.js`
- `wwwroot/lib/FileManager/integration/README-CKEditor-Backup.md`

Tyto soubory obsahují:

- plugin a helpery pro `b64:` parametry
- callback implementaci pro vloení URL zpìt do CKEditoru
- struèné instrukce pro znovupouití

## 8. API pøehled (novı manager)

Base URL: `/api/filemanager`

- `GET /list?path=...&root=...`
- `POST /create-folder`
- `POST /rename`
- `POST /delete`
- `POST /delete-multiple`
- `POST /copy`
- `POST /zip`
- `POST /unzip`
- `GET /thumbnail?path=...&fileName=...&root=...`
- `POST /upload?path=...&root=...`
- `POST /save-text`

Poznámka:

- Query parametry (`path`, `root`, `fileName`) controller dekóduje z `b64:` formátu.

## 9. Co kam kopírovat pøi importu do jiného projektu

## 9.1 Povinné soubory

Backend:

- `Areas/FileManeger/Controllers/FileManagerApiController.cs`
- `Services/CustomFileManagerService.cs`

Frontend:

- `Pages/FileManager.cshtml`
- `Pages/FileManager.cshtml.cs`
- `wwwroot/js/filemanager.js`
- `wwwroot/css/windows-filemanager.css`
- `wwwroot/css/filemanager.css`
- `wwwroot/css/code-editor.css`

Knihovny:

- `wwwroot/lib/ace`
- `wwwroot/lib/file-icon-vectors`
- `wwwroot/lib/bootstrap`
- `wwwroot/lib/jquery`

Legacy èást (pokud ji chceš zachovat):

- `Areas/FileManeger`
- `wwwroot/lib/FileManager`

## 9.2 Ovìøení po kopii

1. Doplò Program.cs (sekce 2).
2. Zkontroluj, e `_Layout.cshtml` naèítá CSS/JS.
3. Spus build a aplikaci.
4. Otevøi `/FileManager`.

## 10. Postup „mám URL na ZIP, chci to nasadit“

Níe je postup pøes PowerShell.

## 10.1 Stáhnout ZIP

```powershell
$zipUrl = 'https://example.com/FileManagerPackage.zip'
$zipPath = "$env:TEMP\FileManagerPackage.zip"
Invoke-WebRequest -Uri $zipUrl -OutFile $zipPath
```

## 10.2 Rozbalit ZIP

```powershell
$extractPath = "$env:TEMP\FileManagerPackage"
if (Test-Path $extractPath) { Remove-Item $extractPath -Recurse -Force }
Expand-Archive -Path $zipPath -DestinationPath $extractPath -Force
```

## 10.3 Nakopírovat do cílového projektu

```powershell
$projectRoot = 'D:\MyProject\WebApplication1'

Copy-Item "$extractPath\Pages\FileManager.cshtml" "$projectRoot\Pages\" -Force
Copy-Item "$extractPath\Pages\FileManager.cshtml.cs" "$projectRoot\Pages\" -Force

Copy-Item "$extractPath\Areas\FileManeger\Controllers\FileManagerApiController.cs" "$projectRoot\Areas\FileManeger\Controllers\" -Force
Copy-Item "$extractPath\Services\CustomFileManagerService.cs" "$projectRoot\Services\" -Force

Copy-Item "$extractPath\wwwroot\js\filemanager.js" "$projectRoot\wwwroot\js\" -Force
Copy-Item "$extractPath\wwwroot\css\windows-filemanager.css" "$projectRoot\wwwroot\css\" -Force
Copy-Item "$extractPath\wwwroot\css\filemanager.css" "$projectRoot\wwwroot\css\" -Force
Copy-Item "$extractPath\wwwroot\css\code-editor.css" "$projectRoot\wwwroot\css\" -Force

Copy-Item "$extractPath\wwwroot\lib\ace" "$projectRoot\wwwroot\lib\" -Recurse -Force
Copy-Item "$extractPath\wwwroot\lib\file-icon-vectors" "$projectRoot\wwwroot\lib\" -Recurse -Force
Copy-Item "$extractPath\wwwroot\lib\bootstrap" "$projectRoot\wwwroot\lib\" -Recurse -Force
Copy-Item "$extractPath\wwwroot\lib\jquery" "$projectRoot\wwwroot\lib\" -Recurse -Force

# volitelnì legacy
if (Test-Path "$extractPath\Areas\FileManeger") {
  Copy-Item "$extractPath\Areas\FileManeger" "$projectRoot\Areas\" -Recurse -Force
}
if (Test-Path "$extractPath\wwwroot\lib\FileManager") {
  Copy-Item "$extractPath\wwwroot\lib\FileManager" "$projectRoot\wwwroot\lib\" -Recurse -Force
}
```

## 10.4 Build a spuštìní

```powershell
dotnet build
dotnet run
```

## 10.5 Kontrola funkènosti

1. Otevøi `/FileManager`.
2. Ovìø listování souborù.
3. Ovìø upload.
4. Ovìø vytvoøení sloky.
5. Ovìø otevøení textového souboru v editoru a uloení.
6. Ovìø picker callback z jiné stránky.

## 11. Nejèastìjší problémy a øešení

1. FileManager se neotevøe:
- zkontroluj route `/FileManager`
- zkontroluj `MapRazorPages()`

2. API vrací 404:
- zkontroluj `MapControllers()`
- zkontroluj pøítomnost `FileManagerApiController`

3. Nejsou vidìt ikony/editor:
- zkontroluj, e existují knihovny v `wwwroot/lib`
- zkontroluj referenci CSS/JS v layoutu

4. Picker nevrací URL:
- callback funkce musí bıt globální (`window.NazevFunkce`)
- popup musí bıt otevøen ze stejné origin domény

5. Root nefunguje:
- root posílej jako `b64:`
- pokud neexistuje, správnì spadne na `UzivatelskeSoubory`

## 12. Doporuèenı standard pro nové implementace

- Vdy pouívej route `/FileManager`
- Vdy pouívej parametry ve formátu `b64:`
- Pro vıbìr souborù pouívej picker reim (`picker=1`) + callback
- Legacy èást v `Areas/FileManeger` ber jen jako kompatibilitní vrstvu


