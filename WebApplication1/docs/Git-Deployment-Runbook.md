# Git Deployment Runbook (Admin FTP)

Tento dokument popisuje kompletní proces nasazení aplikace z Git repozitáře do produkce pomocí interní Admin FTP komponenty.

Platí pro projekt:

- `WebApplication1/WebApplication1`
- runtime: `.NET 8`
- způsob nasazení: build/publish lokálně nebo na build agentu -> upload přes `/Admin` FTP modal
- repozitář: `git@github.com:zukovVeliky/Web_FTP.git`

## 0. Bootstrap na novém stroji

```powershell
git clone git@github.com:zukovVeliky/Web_FTP.git
cd Web_FTP\WebApplication1\WebApplication1
git fetch --all --tags
git checkout main
```

## 1. Architektura nasazení

1. Zdroj kódu je v Gitu.
2. Z vybraného commitu/branch se vytvoří publish output (`dotnet publish`).
3. Admin stránka `/Admin` použije FTP endpointy pro:
   - stop webu (`app_offline.htm`)
   - upload binárek nebo celého webu
   - start webu (odstranění `app_offline.htm`)
4. Po nasazení proběhne smoke test.

## 2. Důležité soubory

- Runtime konfigurace: `appsettings.json`
- Dev konfigurace: `appsettings.Development.json`
- Admin page: `Pages/Admin.cshtml`, `Pages/Admin.cshtml.cs`
- FTP partial: `Areas/FTP/FTP_Update.cshtml`, `Areas/FTP/FTP_Update.cshtml.cs`
- Pipeline a middleware: `Program.cs`

## 3. Konfigurace (Deploy)

V `appsettings.json` je sekce:

```json
"Deploy": {
  "Visible": "True",
  "ProjectName": "Blog.Web",
  "LocalReleasePath": "D:\\AI\\Blog\\Blog.Web\\bin\\Release\\net8.0\\",
  "Ftp": {
    "Host": "ftp-host",
    "Username": "ftp-user",
    "Password": "ftp-password",
    "Port": "21",
    "RemoteRoot": "/subdoms/blog",
    "UseSsl": "false"
  }
}
```

Význam:

1. `Deploy.ProjectName` -> název očekávaných souborů (`.dll/.pdb/.exe`) pro rychlé nasazení.
2. `Deploy.LocalReleasePath` -> default cesta k publish/release output.
3. `Deploy.Ftp.*` -> default FTP připojení předvyplněné v Admin UI.
4. `Deploy.Visible` -> interní příznak viditelnosti nasazení (v současné implementaci se do UI nepromítá podmínkou).

## 4. Bezpečnostní minimum

1. Přístup na `/Admin` jen pro přihlášené uživatele (`[Authorize]`).
2. FTP handlery jsou chráněné anti-forgery tokenem.
3. Neukládat produkční hesla do Gitu:
   - použít User Secrets/CI secrets/KeyVault/ENV proměnné
   - v repozitáři držet jen placeholdery
4. Omezit přístup pouze na roli admin (doporučeno).

## 5. Standardní release postup z Gitu

## 5.1 Připrav release commit

1. Přepni na cílovou větev:

```powershell
git checkout main
git pull
```

2. Ověř, že máš čistý working tree:

```powershell
git status
```

3. Volitelně tag release:

```powershell
git tag vYYYY.MM.DD.N
git push --tags
```

## 5.2 Build a publish

Spusť v kořeni projektu (`WebApplication1/WebApplication1`):

```powershell
dotnet restore
dotnet build -c Release
dotnet publish -c Release -o .\artifacts\publish
```

Poznámka:

1. Pro runtime-specific publish použij `-r win-x64 --self-contained false` podle cílového hostingu.
2. Cestu `.\artifacts\publish` můžeš následně použít jako `LocalReleasePath` v Admin UI.

## 5.3 Lokální kontrola před uploadem

1. Ověř přítomnost hlavních souborů:

```powershell
Get-ChildItem .\artifacts\publish
```

2. Minimálně očekávej:
   - `<ProjectName>.dll`
   - `<ProjectName>.deps.json`
   - `<ProjectName>.runtimeconfig.json`
   - `wwwroot\...`

## 5.4 Nasazení přes Admin UI

1. Přihlas se do aplikace.
2. Otevři `/Admin`.
3. Klikni `Otevřít správu`.
4. Vyplň/zkontroluj:
   - `Host`, `Port`, `Remote root`, `Uživatel`, `Heslo`
   - `Cesta k release složce` (`LocalReleasePath`)
   - `Název projektu` (`ProjectName`)
5. Varianta A: rychlé nasazení binárek
   - tlačítko `Zastavit server a nahrát DLL/PDB/EXE`
6. Varianta B: plný upload webu
   - `Nahrát kompletní web`
   - volby: `Nahrát wwwroot`, `Přepsat existující soubory`
7. Po dokončení zkontroluj stavový panel (`ftpStatus`) bez chyby.

## 6. Post-deploy checklist

1. Otevři homepage.
2. Otevři klíčové stránky:
   - `/`
   - `/FileManager`
   - `/Admin` (jen ověření, že je dostupné po přihlášení)
3. Zkontroluj logy aplikace.
4. Ověř verzi/commit (doporučeno zobrazovat v UI nebo logu při startu).
5. Ověř, že nezůstal `app_offline.htm` na serveru.

## 7. Rollback

## 7.1 Rychlý rollback na předchozí build

1. Měj uložený předchozí publish artifact (adresář nebo ZIP).
2. V `/Admin` proveď:
   - stop webu
   - upload předchozího artifactu (plný upload)
   - start webu

## 7.2 Rollback z Gitu

1. Checkout na předchozí stabilní commit/tag:

```powershell
git checkout <tag-nebo-commit>
dotnet publish -c Release -o .\artifacts\publish-rollback
```

2. Uploadni rollback artifact přes `/Admin`.

## 8. Incident a troubleshooting

## 8.1 FTP upload selhal

1. Ověř `Host/Port/User/Password/RemoteRoot`.
2. Ověř pasivní režim a případně FTPS (`UseSsl`).
3. Ověř oprávnění k zápisu/smazání v cílové složce.

## 8.2 Web po deployi neběží

1. Zkontroluj, zda nezůstal `app_offline.htm`.
2. Zkontroluj kompatibilitu runtime na hostingu.
3. Ověř `appsettings.json` a connection strings.
4. Proveď rollback na poslední funkční artifact.

## 8.3 Chyba 400 na FTP handlerech

1. Ověř, že jsi přihlášen.
2. Ověř anti-forgery token (nesmí být blokován JS/CSP/reverzní proxy).

## 9. Doporučený release checklist (copy/paste)

```text
[ ] Vybraný správný commit/tag
[ ] dotnet restore/build/publish bez chyb
[ ] Artifact obsahuje správné soubory
[ ] Záloha předchozího artifactu připravena
[ ] Nasazení přes /Admin dokončeno bez chyby
[ ] Smoke test prošel
[ ] Logy bez kritických chyb
[ ] rollback plán potvrzen
```

## 10. Doporučené další kroky (budoucí zlepšení)

1. Přesun tajných údajů z `appsettings.json` do secure storage.
2. Role-based autorizace pro `/Admin` (např. `Administrator`).
3. CI pipeline, která bude generovat a archivovat publish artifacty na každý release tag.
4. Auditní log pro deploy operace (kdo, kdy, jaký commit, jaký cíl).
