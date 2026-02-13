using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avatar;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Services.ComponentSettings;

namespace WebApplication1.Pages;

public class AvatarFileManagerModel : PageModel
{
    private const int DefaultMaxCropStateLength = 2000;
    private const int DefaultMaxDisplayedLogEntries = 100;
    private const string AvatarInstanceId = "avatar-main";
    public sealed class CropLogEntry
    {
        public string Line { get; init; } = string.Empty;
        public string? ImageUrl { get; init; }
    }

    private readonly IWebHostEnvironment _environment;
    private readonly IComponentSettingsProvider _componentSettings;

    public AvatarFileManagerModel(IWebHostEnvironment environment, IComponentSettingsProvider componentSettings)
    {
        _environment = environment;
        _componentSettings = componentSettings;
    }

    public EditaceAvataraModel AvatarEditor { get; private set; } = default!;

    public List<CropLogEntry> CropLogLines { get; private set; } = new();

    public string? LastSavedCropUrl { get; private set; }

    public AvatarComponentSettings Settings { get; private set; } = new();

    public string CallbackFunctionName => $"ZachyceniURL_{Settings.Modifier}";

    private string LogFilePath =>
        Path.Combine(_environment.ContentRootPath, "App_Data", "avatar-crop-records.txt");

    public void OnGet()
    {
        Settings = GetSettings();
        AvatarEditor = BuildAvatarEditor();
        CropLogLines = ReadLogLines();
    }

    public IActionResult OnPostSave()
    {
        Settings = GetSettings();

        var hiddenFieldName = $"HiddenParametry_{Settings.Modifier}";
        var cropState = Request.Form[hiddenFieldName].ToString();

        if (string.IsNullOrWhiteSpace(cropState))
        {
            ModelState.AddModelError(string.Empty, "Chybi data o orezani obrazku.");
            AvatarEditor = BuildAvatarEditor();
            CropLogLines = ReadLogLines();
            return Page();
        }
        var maxCropStateLength = ClampPositive(Settings.MaxCropStateLength, DefaultMaxCropStateLength);
        if (cropState.Length > maxCropStateLength)
        {
            ModelState.AddModelError(string.Empty, "Data orezu jsou prilis dlouha.");
            AvatarEditor = BuildAvatarEditor();
            CropLogLines = ReadLogLines();
            return Page();
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        try
        {
            var result = AvatarCropp.GetNewAvatar(_environment.WebRootPath, baseUrl, cropState);
            LastSavedCropUrl = result.FullURL;

            AppendLogLine(cropState, result.FullURL);

            // Keep the currently loaded source image and crop state in the editor
            // so user can continue adjusting the same image without selecting again.
            AvatarEditor = BuildAvatarEditor(cropState);
            CropLogLines = ReadLogLines();
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Ulozeni selhalo: {ex.Message}");
            AvatarEditor = BuildAvatarEditor(cropState);
            CropLogLines = ReadLogLines();
            return Page();
        }
    }

    private EditaceAvataraModel BuildAvatarEditor(string? cropState = null)
    {
        var width = ClampPositive(Settings.CropWidth, 800).ToString();
        var height = ClampPositive(Settings.CropHeight, 200).ToString();

        if (string.IsNullOrWhiteSpace(cropState))
        {
            return new EditaceAvataraModel(
                _environment.WebRootPath,
                Request,
                ZobrazeniCtverce: true,
                width: width,
                height: height,
                modifikator: Settings.Modifier);
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        return new EditaceAvataraModel(
            _environment.WebRootPath,
            Request,
            cropState,
            baseUrl,
            ZobrazeniCtverce: true,
            width: width,
            height: height,
            modifikator: Settings.Modifier);
    }

    private void AppendLogLine(string cropState, string croppedRelativeUrl)
    {
        var values = cropState.Split('~');
        string SafeAt(int index) => index >= 0 && index < values.Length ? values[index] : "0";

        var line = string.Join(" | ", new[]
        {
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            $"source={SafeAt(0)}",
            $"x0={SafeAt(1)}",
            $"y0={SafeAt(2)}",
            $"x1={SafeAt(3)}",
            $"y1={SafeAt(4)}",
            $"zoom={SafeAt(5)}",
            $"viewportW={SafeAt(6)}",
            $"viewportH={SafeAt(7)}",
            $"cropped={croppedRelativeUrl}"
        });

        var directory = Path.GetDirectoryName(LogFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        System.IO.File.AppendAllLines(LogFilePath, new[] { line });
    }

    private List<CropLogEntry> ReadLogLines()
    {
        if (!System.IO.File.Exists(LogFilePath))
        {
            return new List<CropLogEntry>();
        }
        var maxDisplayed = ClampPositive(Settings.MaxDisplayedLogEntries, DefaultMaxDisplayedLogEntries);

        return System.IO.File
            .ReadAllLines(LogFilePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Reverse()
            .Take(maxDisplayed)
            .Select(line => new CropLogEntry
            {
                Line = line,
                ImageUrl = ExtractCroppedImageUrl(line)
            })
            .ToList();
    }

    private AvatarComponentSettings GetSettings()
    {
        return _componentSettings.GetAvatarSettings(AvatarInstanceId, Request.Path);
    }

    private static int ClampPositive(int value, int fallback)
    {
        return value > 0 ? value : fallback;
    }

    private static string? ExtractCroppedImageUrl(string line)
    {
        const string key = "cropped=";
        var part = line
            .Split(" | ", StringSplitOptions.TrimEntries)
            .FirstOrDefault(x => x.StartsWith(key, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(part))
        {
            return null;
        }

        var value = part.Substring(key.Length).Trim();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("/", StringComparison.Ordinal))
        {
            return value;
        }

        return "/" + value.TrimStart('/');
    }

}
