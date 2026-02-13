using CKEditor5;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using WebApplication1.Services.ComponentSettings;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IComponentSettingsProvider _componentSettings;
        private const string DefaultText = "<p>Ukazka textoveho editoru.</p>";
        private const string CKEditorInstanceIdMain = "home-editor-main";
        private const string CKEditorInstanceIdSecondary1 = "home-editor-secondary-1";
        private const string CKEditorInstanceIdSecondary2 = "home-editor-secondary-2";

        public IndexModel(ILogger<IndexModel> logger, IComponentSettingsProvider componentSettings)
        {
            _logger = logger;
            _componentSettings = componentSettings;
        }

        public string Obsah { get; private set; } = string.Empty;
        public CKEditor5Model EditorMain { get; private set; } = null!;
        public CKEditor5Model EditorSecondary1 { get; private set; } = null!;
        public CKEditor5Model EditorSecondary2 { get; private set; } = null!;

        public void OnGet()
        {
            Obsah = DefaultText;
            BuildEditors(Obsah);
        }

        public void OnPostEditText([FromForm] string? ckeditor5)
        {
            Obsah = string.IsNullOrWhiteSpace(ckeditor5) ? DefaultText : ckeditor5;
            BuildEditors(Obsah);
        }

        private void BuildEditors(string mainText)
        {
            EditorMain = BuildEditor(CKEditorInstanceIdMain, mainText, "ZachyceniURLCKeditor_main");
            EditorSecondary1 = BuildEditor(CKEditorInstanceIdSecondary1, "<p>Druhy editor na stejne strance.</p>", "ZachyceniURLCKeditor_secondary_1");
            EditorSecondary2 = BuildEditor(CKEditorInstanceIdSecondary2, "<p>Treti editor na stejne strance.</p>", "ZachyceniURLCKeditor_secondary_2");
        }

        private CKEditor5Model BuildEditor(string instanceId, string text, string fallbackCallbackName)
        {
            var settings = _componentSettings.GetCKEditorSettings(instanceId, Request.Path);
            var setPath = Convert.ToBase64String(Encoding.UTF8.GetBytes(Path.Combine(settings.SetPath)));
            var callbackName = string.IsNullOrWhiteSpace(settings.CallbackName)
                ? fallbackCallbackName
                : settings.CallbackName + "_" + instanceId.Replace('-', '_');

            return new CKEditor5Model(
                text,
                setPath,
                Request,
                instanceId: instanceId,
                callbackName: callbackName,
                pickerAllowExt: settings.PickerAllowExt,
                pickerPopupName: settings.PopupName);
        }
    }
}
