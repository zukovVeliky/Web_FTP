using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Avatar;

public class AvatarCropp
{
    private const int MinOutputSize = 1;
    private const int MaxOutputSize = 4000;

    private sealed class CropState
    {
        public string SourceUrl { get; init; } = string.Empty;
        public double X0 { get; init; }
        public double Y0 { get; init; }
        public double Zoom { get; init; }
        public double ViewportWidth { get; init; }
        public double ViewportHeight { get; init; }
    }

    public static AvatarIMG GetNewAvatar(string webRoot, string urlBase, string stateString)
    {
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            throw new ArgumentException("Chybi webRoot.");
        }

        var state = ParseState(stateString);
        var sourcePath = ResolveSourcePath(webRoot, urlBase, state.SourceUrl);

        var fileName = Guid.NewGuid() + ".png";
        var outputDirectory = Path.Combine(webRoot, "Images", "Foto_Avatar", "Cropped");
        Directory.CreateDirectory(outputDirectory);

        using var image = Image.FromFile(sourcePath);
        ApplyExifOrientation(image);

        var outputWidth = ClampInt((int)Math.Round(state.ViewportWidth), MinOutputSize, MaxOutputSize);
        var outputHeight = ClampInt((int)Math.Round(state.ViewportHeight), MinOutputSize, MaxOutputSize);

        var zoom = state.Zoom <= 0 ? 1d : state.Zoom;
        var sourceCropWidth = state.ViewportWidth / zoom;
        var sourceCropHeight = state.ViewportHeight / zoom;

        var imgWidth = (double)image.Width;
        var imgHeight = (double)image.Height;

        var x = ClampDouble(state.X0, 0, Math.Max(0, imgWidth - 1));
        var y = ClampDouble(state.Y0, 0, Math.Max(0, imgHeight - 1));
        var width = ClampDouble(sourceCropWidth, 1, Math.Max(1, imgWidth - x));
        var height = ClampDouble(sourceCropHeight, 1, Math.Max(1, imgHeight - y));

        var sourceRect = Rectangle.FromLTRB(
            (int)Math.Floor(x),
            (int)Math.Floor(y),
            (int)Math.Ceiling(x + width),
            (int)Math.Ceiling(y + height));

        if (sourceRect.Width <= 0 || sourceRect.Height <= 0)
        {
            sourceRect = new Rectangle(0, 0, image.Width, image.Height);
        }

        using (var croppedImage = new Bitmap(outputWidth, outputHeight))
        using (var graphics = Graphics.FromImage(croppedImage))
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, new Rectangle(0, 0, outputWidth, outputHeight), sourceRect, GraphicsUnit.Pixel);
            croppedImage.Save(Path.Combine(outputDirectory, fileName));
        }

        TryDeletePreviousCroppedSource(webRoot, urlBase, state.SourceUrl);

        var relativeUrl = "Images/Foto_Avatar/Cropped/" + fileName;
        return new AvatarIMG(fileName, outputDirectory, relativeUrl);
    }

    private static CropState ParseState(string stateString)
    {
        if (string.IsNullOrWhiteSpace(stateString))
        {
            throw new ArgumentException("Chybi crop data.");
        }

        var values = stateString.Split('~');
        if (values.Length < 8)
        {
            throw new ArgumentException("Crop data nemaji ocekavany format.");
        }

        return new CropState
        {
            SourceUrl = values[0],
            X0 = ParseDouble(values[1], "x0"),
            Y0 = ParseDouble(values[2], "y0"),
            Zoom = ParseDouble(values[5], "zoom"),
            ViewportWidth = ParseDouble(values[6], "viewport width"),
            ViewportHeight = ParseDouble(values[7], "viewport height")
        };
    }

    private static double ParseDouble(string value, string label)
    {
        var normalized = (value ?? "0").Trim().Replace(',', '.');
        if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
        {
            throw new ArgumentException($"Neplatna hodnota pro {label}.");
        }

        return parsed;
    }

    private static void ApplyExifOrientation(Image image)
    {
        try
        {
            var orientationItem = image.PropertyItems.FirstOrDefault(p => p.Id == 0x112);
            if (orientationItem?.Value is null || orientationItem.Value.Length < 2)
            {
                return;
            }

            var orientation = BitConverter.ToUInt16(orientationItem.Value, 0);
            switch (orientation)
            {
                case 3:
                    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 6:
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 8:
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
        }
        catch
        {
            // EXIF orientace je optional. Pokud selze, crop pokracuje bez rotace.
        }
    }

    private static string ResolveSourcePath(string webRoot, string urlBase, string sourceUrl)
    {
        var relative = NormalizeToRelativePath(sourceUrl, urlBase);
        if (string.IsNullOrWhiteSpace(relative))
        {
            relative = "Images/Foto_Avatar/1000.jpg";
        }

        var fullPath = Path.GetFullPath(Path.Combine(webRoot, relative.Replace("/", Path.DirectorySeparatorChar.ToString())));
        var normalizedRoot = Path.GetFullPath(webRoot).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Neplatna cesta ke zdrojovemu obrazku.");
        }

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Zdrojovy obrazek nebyl nalezen.", fullPath);
        }

        return fullPath;
    }

    private static string NormalizeToRelativePath(string sourceUrl, string urlBase)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl))
        {
            return string.Empty;
        }

        var normalized = sourceUrl.Trim().Replace("\\", "/");

        if (Uri.TryCreate(normalized, UriKind.Absolute, out var absolute))
        {
            normalized = absolute.AbsolutePath;
        }
        else if (!string.IsNullOrWhiteSpace(urlBase) &&
                 normalized.StartsWith(urlBase, StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized.Substring(urlBase.Length);
        }

        return normalized.TrimStart('/');
    }

    private static void TryDeletePreviousCroppedSource(string webRoot, string urlBase, string sourceUrl)
    {
        var relative = NormalizeToRelativePath(sourceUrl, urlBase);
        if (!relative.StartsWith("Images/Foto_Avatar/Cropped/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            var fullPath = ResolveSourcePath(webRoot, urlBase, relative);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch
        {
            // Mazani predchoziho cropu je best-effort.
        }
    }

    private static int ClampInt(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    private static double ClampDouble(double value, double min, double max)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return min;
        }

        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}

public class AvatarIMG
{
    public string Name { get; private set; }
    public string Path { get; private set; }
    public string FullURL { get; private set; }

    public AvatarIMG(string fileName, string path, string fullURL)
    {
        Name = fileName;
        Path = path;
        FullURL = fullURL;
    }
}
