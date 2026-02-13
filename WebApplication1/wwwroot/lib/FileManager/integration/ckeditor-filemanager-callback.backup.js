/*
  Backup: callback implementace pouzita v CKEditoru.
  Tuto funkci vola FileManager picker v window.opener.
*/
function ZachyceniURLCKeditor(zachyceneURL, fileName) {
    if (!window.CKEditor || !zachyceneURL) {
        return;
    }

    var selection = CKEditor.model.document.selection;
    if (selection && !selection.isCollapsed) {
        CKEditor.execute('link', zachyceneURL);
        return;
    }

    var resolvedFileName = fileName || getFileNameFromUrl(zachyceneURL) || zachyceneURL;
    if (/\.(png|jpe?g|gif|bmp|webp|svg)$/i.test(String(resolvedFileName || '').toLowerCase())) {
        CKEditor.execute('insertImage', { source: zachyceneURL });
        return;
    }

    CKEditor.model.change(function (writer) {
        var insertAt = CKEditor.model.document.selection.getFirstPosition();
        writer.insertText(resolvedFileName, { linkHref: zachyceneURL }, insertAt);
    });
}

function getFileNameFromUrl(url) {
    try {
        var parsed = new URL(url, window.location.origin);
        var path = parsed.pathname || '';
        var idx = path.lastIndexOf('/');
        return idx >= 0 ? decodeURIComponent(path.substring(idx + 1)) : decodeURIComponent(path);
    } catch (e) {
        return '';
    }
}
