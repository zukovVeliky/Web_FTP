using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Services.ComponentSettings;

namespace WebApplication1.Pages;

public class FileManagerModel : PageModel
{
    private const string FileManagerInstanceId = "main-filemanager-page";
    private readonly IComponentSettingsProvider _componentSettings;

    public FileManagerModel(IComponentSettingsProvider componentSettings)
    {
        _componentSettings = componentSettings;
    }

    public FileManagerComponentSettings Settings { get; private set; } = new();

    public void OnGet()
    {
        Settings = _componentSettings.GetFileManagerSettings(FileManagerInstanceId, Request.Path);
    }
}
