# Zaloha integrace CKEditor + FileManager

Soubory v teto slozce jsou zaloha implementace, ktera je aktualne pouzita v:

- `Areas/CKEditor5/Pages/CKEditor5.cshtml`

Obsah:

- `ckeditor-filemanager-plugin.backup.js`
  - toolbar plugin (`fileManager`, `fileManagerAll`)
  - otevirani `/FileManager` pickeru
  - helpery pro `b64:` kodovani parametru

- `ckeditor-filemanager-callback.backup.js`
  - callback `ZachyceniURLCKeditor(url, fileName)`
  - vlozeni obrazku nebo odkazu do CKEditoru
