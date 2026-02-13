/*
  Backup: CKEditor 5 integrace FileManageru z projektu.
  Pouziti:
  1) nacti tento soubor na strance s CKEditorem
  2) zavolej createFileManagerToolbarPlugin({ setPath: '...' })
  3) pridej plugin do extraPlugins a toolbar itemy fileManager / fileManagerAll
*/
(function (global) {
  function createFileManagerToolbarPlugin(options) {
    options = options || {};

    var FILE_MANAGER_ICON = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"><path d="M1.201 1c-.662 0-1.2.47-1.2 1.1v14.248c0 .64.533 1.152 1.185 1.152h6.623v-7.236L6.617 9.15a.694.694 0 0 0-.957-.033L1.602 13.55V2.553l14.798.003V9.7H18V2.1c0-.63-.547-1.1-1.2-1.1zm11.723 2.805a2.1 2.1 0 0 0-1.621.832 2.127 2.127 0 0 0 1.136 3.357 2.13 2.13 0 0 0 2.611-1.506 2.13 2.13 0 0 0-.76-2.244 2.13 2.13 0 0 0-1.366-.44Z"/><path d="M19.898 12.369v6.187a.844.844 0 0 1-.844.844h-8.719a.844.844 0 0 1-.843-.844v-7.312a.844.844 0 0 1 .843-.844h2.531a.84.84 0 0 1 .597.248l.838.852h4.75c.223 0 .441.114.6.272a.84.84 0 0 1 .247.597m-1.52.654-4.377.02-1.1-1.143H11v6h7.4l-.023-4.877Z"/></svg>';
    var FILE_MANAGER_ALL_ICON = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"><path d="M4.2 3c-.584 0-1.145.23-1.557.643A2.2 2.2 0 0 0 2 5.199v8.719a2.194 2.194 0 0 0 2.2 2.195h11.624a2.194 2.194 0 0 0 2.196-2.195V7.621a2.194 2.194 0 0 0-2.195-2.2h-5.393l-1.237-2.06A.75.75 0 0 0 8.56 3zm0 1.488h3.935l1.236 2.06a.75.75 0 0 0 .64.362h5.813a.71.71 0 0 1 .707.71v6.298a.707.707 0 0 1-.707.707H4.2a.71.71 0 0 1-.71-.707V5.199a.71.71 0 0 1 .71-.71Z"></path></svg>';

    function resolveButtonViewConstructor(editor) {
      var candidates = ['undo', 'redo', 'bold', 'italic', 'link'];
      for (var i = 0; i < candidates.length; i++) {
        try {
          var seed = editor.ui.componentFactory.create(candidates[i]);
          if (seed && seed.constructor) {
            var ctor = seed.constructor;
            if (typeof seed.destroy === 'function') seed.destroy();
            return ctor;
          }
        } catch (e) { }
      }
      return null;
    }

    function openPicker(setPath, allFiles) {
      var root = decodeFileManagerParam(setPath || '');
      var url = '/FileManager'
        + '?picker=' + encodeURIComponent(encodeFileManagerParam('1'))
        + '&callback=' + encodeURIComponent(encodeFileManagerParam('ZachyceniURLCKeditor'))
        + '&root=' + encodeURIComponent(encodeFileManagerParam(root));

      if (!allFiles) {
        url += '&allowExt=' + encodeURIComponent(encodeFileManagerParam('jpg,jpeg,png,gif,bmp,webp,svg,pdf'));
      }

      global.open(url, 'FileManagerPicker', 'width=1280,height=820,resizable=yes,scrollbars=yes');
    }

    function Plugin(editor) {
      this.editor = editor;
    }

    Plugin.prototype.init = function () {
      var editor = this.editor;
      var ButtonViewCtor = resolveButtonViewConstructor(editor);
      if (!ButtonViewCtor) return;

      editor.ui.componentFactory.add('fileManager', function (locale) {
        var button = new ButtonViewCtor(locale);
        button.set({ label: 'Souborovy manager', icon: FILE_MANAGER_ICON, tooltip: true, withText: false });
        button.on('execute', function () { openPicker(options.setPath, false); });
        return button;
      });

      editor.ui.componentFactory.add('fileManagerAll', function (locale) {
        var button = new ButtonViewCtor(locale);
        button.set({ label: 'Souborovy manager (vse)', icon: FILE_MANAGER_ALL_ICON, tooltip: true, withText: false });
        button.on('execute', function () { openPicker(options.setPath, true); });
        return button;
      });
    };

    Plugin.pluginName = 'FileManagerToolbarPlugin';
    return Plugin;
  }

  function encodeFileManagerParam(value) {
    var bytes = new TextEncoder().encode(String(value == null ? '' : value));
    var binary = '';
    for (var i = 0; i < bytes.length; i++) binary += String.fromCharCode(bytes[i]);
    return 'b64:' + btoa(binary).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/g, '');
  }

  function decodeFileManagerParam(value) {
    var raw = String(value == null ? '' : value).trim();
    if (!raw) return '';
    if (raw.indexOf('b64:') === 0) {
      try {
        var normalized = raw.substring(4).replace(/-/g, '+').replace(/_/g, '/');
        var padLen = normalized.length % 4 === 0 ? 0 : 4 - (normalized.length % 4);
        var padded = normalized + '='.repeat(padLen);
        var binary = atob(padded);
        var bytes = new Uint8Array(binary.length);
        for (var i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
        return new TextDecoder().decode(bytes);
      } catch (e) { return ''; }
    }
    return raw;
  }

  global.FileManagerCkEditorIntegration = {
    createFileManagerToolbarPlugin: createFileManagerToolbarPlugin,
    encodeFileManagerParam: encodeFileManagerParam,
    decodeFileManagerParam: decodeFileManagerParam
  };
})(window);
