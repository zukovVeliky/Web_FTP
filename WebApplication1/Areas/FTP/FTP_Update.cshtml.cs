
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Reflection;
using System.Text;

namespace FTP
{

    [Authorize]
    public class FTP_UpdateModel : PageModel
    {
        
        public string DefaultProjectName { get; private set; } = "";
        public string DefaultReleasePath { get; private set; } = "";
        public string DefaultFtpHost { get; private set; } = "";
        public string DefaultFtpUsername { get; private set; } = "";
        public string DefaultFtpPassword { get; private set; } = "";
        public string DefaultFtpRemoteRoot { get; private set; } = "/";
        public int DefaultFtpPort { get; private set; } = 21;
        public bool DefaultFtpUseSsl { get; private set; }
        public bool DeployVisible { get; private set; } = true;
        

        //private readonly BlogDbContext _db;
        private readonly IConfiguration _configuration;
        //private readonly IWebHostEnvironment _env;
        public FTP_UpdateModel(IConfiguration con)
        {
            _configuration = con;

        }

        public async Task OnGetAsync()
        {
            DefaultProjectName = _configuration["Deploy:ProjectName"]
                ?? Assembly.GetEntryAssembly()?.GetName().Name
                ?? "Blog.Web";
            DefaultReleasePath = _configuration["Deploy:LocalReleasePath"] ?? string.Empty;
            DefaultFtpHost = _configuration["Deploy:Ftp:Host"] ?? string.Empty;
            DefaultFtpUsername = _configuration["Deploy:Ftp:Username"] ?? string.Empty;
            DefaultFtpPassword = _configuration["Deploy:Ftp:Password"] ?? string.Empty;
            DefaultFtpRemoteRoot = _configuration["Deploy:Ftp:RemoteRoot"] ?? "/";
            DefaultFtpPort = int.TryParse(_configuration["Deploy:Ftp:Port"], out var port) ? port : 21;
            DefaultFtpUseSsl = string.Equals(_configuration["Deploy:Ftp:UseSsl"], "true", StringComparison.OrdinalIgnoreCase);
            DeployVisible = IsDeployVisible();

        }

        public async Task<IActionResult> OnPostFtpDeployAsync([FromForm] FTP.FTP_UpdateModel.FtpDeployRequest request)
        {
            if (!TryValidateFtpRequest(request, out var error))
            {
                return new JsonResult(new { success = false, message = error }) { StatusCode = 400 };
            }

            var projectName = string.IsNullOrWhiteSpace(request.ProjectName)
                ? GetDefaultProjectName()
                : request.ProjectName.Trim();

            var releasePath = string.IsNullOrWhiteSpace(request.LocalReleasePath)
                ? GetDefaultReleasePath()
                : request.LocalReleasePath.Trim();

            if (string.IsNullOrWhiteSpace(projectName))
            {
                return new JsonResult(new { success = false, message = "Chyb� n�zev projektu." }) { StatusCode = 400 };
            }

            if (string.IsNullOrWhiteSpace(releasePath) || !Directory.Exists(releasePath))
            {
                return new JsonResult(new { success = false, message = "Cesta k release slo�ce neexistuje." }) { StatusCode = 400 };
            }

            var files = new[]
            {
                $"{projectName}.dll",
                $"{projectName}.pdb",
                $"{projectName}.exe"
            };

            var missing = new List<string>();
            var filePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in files)
            {
                var fullPath = Path.Combine(releasePath, file);
                if (!System.IO.File.Exists(fullPath))
                {
                    missing.Add(file);
                }
                else
                {
                    filePaths[file] = fullPath;
                }
            }

            if (missing.Count > 0)
            {
                return new JsonResult(new { success = false, message = $"Chyb� soubory: {string.Join(", ", missing)}" }) { StatusCode = 400 };
            }

            try
            {
                var offlineContent = $"<html><body><h1>Server is being updated</h1><p>{DateTimeOffset.UtcNow:O}</p></body></html>";
                await UploadTextAsync(request, CombineRemotePath(request.RemoteRoot, "app_offline.htm"), offlineContent);

                foreach (var file in filePaths)
                {
                    await UploadFileAsync(request, CombineRemotePath(request.RemoteRoot, file.Key), file.Value);
                }

                var deleteResult = await TryDeleteFileAsync(request, CombineRemotePath(request.RemoteRoot, "app_offline.htm"));
                var message = deleteResult
                    ? "Nasazen� dokon�eno."
                    : "Nasazen� dokon�eno, ale nepoda�ilo se odstranit app_offline.htm.";

                return new JsonResult(new { success = true, message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"FTP nasazen� selhalo: {ex.Message}" }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> OnPostFtpReadConfigAsync([FromForm] FtpConfigRequest request)
        {
            if (!TryValidateFtpRequest(request, out var error))
            {
                return new JsonResult(new { success = false, message = error }) { StatusCode = 400 };
            }

            if (!IsAllowedConfigFile(request.FileName))
            {
                return new JsonResult(new { success = false, message = "Nepodporovan� soubor." }) { StatusCode = 400 };
            }

            try
            {
                var content = await DownloadTextAsync(request, CombineRemotePath(request.RemoteRoot, request.FileName));
                return new JsonResult(new { success = true, content });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Na�ten� selhalo: {ex.Message}" }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> OnPostFtpWriteConfigAsync([FromForm] FtpConfigRequest request)
        {
            if (!TryValidateFtpRequest(request, out var error))
            {
                return new JsonResult(new { success = false, message = error }) { StatusCode = 400 };
            }

            if (!IsAllowedConfigFile(request.FileName))
            {
                return new JsonResult(new { success = false, message = "Nepodporovan� soubor." }) { StatusCode = 400 };
            }

            if (request.Content == null)
            {
                return new JsonResult(new { success = false, message = "Chyb� obsah souboru." }) { StatusCode = 400 };
            }

            try
            {
                await UploadTextAsync(request, CombineRemotePath(request.RemoteRoot, request.FileName), request.Content);
                return new JsonResult(new { success = true, message = "Soubor byl aktualizov�n." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Ulo�en� selhalo: {ex.Message}" }) { StatusCode = 500 };
            }
        }

        private static bool IsAllowedConfigFile(string? fileName)
        {
            return string.Equals(fileName, "appsettings.json", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fileName, "web.config", StringComparison.OrdinalIgnoreCase);
        }

        private string GetDefaultProjectName()
        {
            return _configuration["Deploy:ProjectName"]
                ?? Assembly.GetEntryAssembly()?.GetName().Name
                ?? "Blog.Web";
        }

        private string GetDefaultReleasePath()
        {
            return _configuration["Deploy:LocalReleasePath"] ?? string.Empty;
        }

        private bool IsDeployVisible()
        {
            var visibleValue = _configuration["Deploy:Visible"];
            if (string.IsNullOrWhiteSpace(visibleValue))
            {
                return true;
            }

            if (bool.TryParse(visibleValue, out var visible))
            {
                return visible;
            }

            return !string.Equals(visibleValue, "0", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryValidateFtpRequest(FtpRequestBase request, out string error)
        {
            if (string.IsNullOrWhiteSpace(request.Host))
            {
                error = "Chyb� FTP host.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                error = "Chyb� FTP u�ivatel.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                error = "Chyb� FTP heslo.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static string CombineRemotePath(string? root, string fileName)
        {
            var trimmedRoot = (root ?? string.Empty).Replace("\\", "/").Trim('/');
            if (string.IsNullOrWhiteSpace(trimmedRoot))
            {
                return fileName;
            }
            return $"{trimmedRoot}/{fileName}";
        }

        private static Uri BuildFtpUri(FtpRequestBase settings, string remotePath)
        {
            var host = settings.Host.Trim();
            if (host.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
            {
                host = host.Substring("ftp://".Length);
            }

            var port = settings.Port > 0 ? settings.Port : 21;
            var path = remotePath.Replace("\\", "/").TrimStart('/');
            var builder = new UriBuilder("ftp", host, port, path);
            return builder.Uri;
        }

        private static FtpWebRequest CreateRequest(FtpRequestBase settings, string remotePath, string method)
        {
            var request = (FtpWebRequest)WebRequest.Create(BuildFtpUri(settings, remotePath));
            request.Method = method;
            request.Credentials = new NetworkCredential(settings.Username, settings.Password);
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = false;
            request.EnableSsl = settings.UseSsl;
            return request;
        }

        private static async Task UploadFileAsync(FtpRequestBase settings, string remotePath, string localPath)
        {
            var request = CreateRequest(settings, remotePath, WebRequestMethods.Ftp.UploadFile);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(localPath);
            request.ContentLength = fileBytes.Length;
            await using (var requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
            using var response = (FtpWebResponse)await request.GetResponseAsync();
        }

        private static async Task UploadTextAsync(FtpRequestBase settings, string remotePath, string content)
        {
            var request = CreateRequest(settings, remotePath, WebRequestMethods.Ftp.UploadFile);
            var bytes = Encoding.UTF8.GetBytes(content ?? string.Empty);
            request.ContentLength = bytes.Length;
            await using (var requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(bytes, 0, bytes.Length);
            }
            using var response = (FtpWebResponse)await request.GetResponseAsync();
        }

        private static async Task<string> DownloadTextAsync(FtpRequestBase settings, string remotePath)
        {
            var request = CreateRequest(settings, remotePath, WebRequestMethods.Ftp.DownloadFile);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            await using var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                return string.Empty;
            }
            using var reader = new StreamReader(responseStream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        private static async Task<bool> TryDeleteFileAsync(FtpRequestBase settings, string remotePath)
        {
            try
            {
                var request = CreateRequest(settings, remotePath, WebRequestMethods.Ftp.DeleteFile);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static async Task DeleteRemoteTreeAsync(FtpRequestBase settings, string remoteRoot, bool keepWwwroot)
        {
            var normalizedRoot = remoteRoot?.Trim().Trim('/');
            var rootPath = string.IsNullOrWhiteSpace(normalizedRoot) ? string.Empty : normalizedRoot;
            await DeleteDirectoryContentsAsync(settings, rootPath, keepWwwroot);
        }

        private static async Task DeleteDirectoryContentsAsync(FtpRequestBase settings, string remotePath, bool keepWwwroot)
        {
            var entries = await ListDirectoryAsync(settings, remotePath);
            foreach (var entry in entries)
            {
                if (keepWwwroot && string.Equals(entry.Name, "wwwroot", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var childPath = CombineRemotePath(remotePath, entry.Name);
                if (entry.IsDirectory)
                {
                    await DeleteDirectoryContentsAsync(settings, childPath, false);
                    await TryRemoveDirectoryAsync(settings, childPath);
                }
                else
                {
                    await TryDeleteFileAsync(settings, childPath);
                }
            }
        }

        private static async Task<bool> TryRemoveDirectoryAsync(FtpRequestBase settings, string remotePath)
        {
            try
            {
                var request = CreateRequest(settings, remotePath, WebRequestMethods.Ftp.RemoveDirectory);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<List<FtpEntry>> ListDirectoryAsync(FtpRequestBase settings, string remotePath)
        {
            var request = CreateRequest(settings, remotePath, WebRequestMethods.Ftp.ListDirectoryDetails);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            await using var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                return new List<FtpEntry>();
            }

            using var reader = new StreamReader(responseStream, Encoding.UTF8);
            var entries = new List<FtpEntry>();
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (TryParseWindowsListLine(line, out var winEntry))
                {
                    entries.Add(winEntry);
                    continue;
                }

                if (TryParseUnixListLine(line, out var unixEntry))
                {
                    entries.Add(unixEntry);
                }
            }

            return entries;
        }

        private static bool TryParseWindowsListLine(string line, out FtpEntry entry)
        {
            entry = default;
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4)
            {
                return false;
            }

            if (!DateTime.TryParse($"{parts[0]} {parts[1]}", out _))
            {
                return false;
            }

            var isDir = string.Equals(parts[2], "<DIR>", StringComparison.OrdinalIgnoreCase);
            var name = string.Join(" ", parts.Skip(3));
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            entry = new FtpEntry(name, isDir);
            return true;
        }

        private static bool TryParseUnixListLine(string line, out FtpEntry entry)
        {
            entry = default;
            if (line.Length < 10)
            {
                return false;
            }

            var isDir = line[0] == 'd';
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 9)
            {
                return false;
            }

            var name = string.Join(" ", parts.Skip(8));
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            entry = new FtpEntry(name, isDir);
            return true;
        }

        private static async Task UploadDirectoryAsync(FtpRequestBase settings, string localRoot, string remoteRoot, bool includeWwwroot, bool overwriteExisting)
        {
            var basePath = Path.GetFullPath(localRoot);
            foreach (var directory in Directory.GetDirectories(basePath, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(basePath, directory);
                if (!includeWwwroot && IsUnderWwwroot(relative))
                {
                    continue;
                }

                var remotePath = CombineRemotePath(remoteRoot, NormalizeRelativePath(relative));
                await EnsureDirectoryAsync(settings, remotePath);
            }

            foreach (var file in Directory.GetFiles(basePath, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(basePath, file);
                if (!includeWwwroot && IsUnderWwwroot(relative))
                {
                    continue;
                }

                var remotePath = CombineRemotePath(remoteRoot, NormalizeRelativePath(relative));
                if (!overwriteExisting)
                {
                    var exists = await RemoteFileExistsAsync(settings, remotePath);
                    if (exists)
                    {
                        continue;
                    }
                }

                await UploadFileAsync(settings, remotePath, file);
            }
        }

        private static bool IsUnderWwwroot(string relativePath)
        {
            var normalized = relativePath.Replace('\\', '/').TrimStart('/');
            return normalized.StartsWith("wwwroot/", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(normalized, "wwwroot", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeRelativePath(string relativePath)
        {
            return relativePath.Replace('\\', '/').TrimStart('/');
        }

        private static async Task EnsureDirectoryAsync(FtpRequestBase settings, string remotePath)
        {
            try
            {
                var request = CreateRequest(settings, remotePath, WebRequestMethods.Ftp.MakeDirectory);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
            }
            catch
            {
                // Ignore if exists.
            }
        }

        private static async Task<bool> RemoteFileExistsAsync(FtpRequestBase settings, string remotePath)
        {
            try
            {
                var request = CreateRequest(settings, remotePath, WebRequestMethods.Ftp.GetFileSize);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private readonly record struct FtpEntry(string Name, bool IsDirectory);

        public class FtpRequestBase
        {
            public string Host { get; set; } = string.Empty;
            public int Port { get; set; } = 21;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string RemoteRoot { get; set; } = "/";
            public bool UseSsl { get; set; }
        }

        public class FtpDeployRequest : FtpRequestBase
        {
            public string ProjectName { get; set; } = string.Empty;
            public string LocalReleasePath { get; set; } = string.Empty;
        }

        public class FtpConfigRequest : FtpRequestBase
        {
            public string FileName { get; set; } = string.Empty;
            public string? Content { get; set; }
        }

        public class FtpDeleteRequest : FtpRequestBase
        {
            public bool KeepWwwroot { get; set; }
        }

        public class FtpUploadRequest : FtpRequestBase
        {
            public string LocalReleasePath { get; set; } = string.Empty;
            public bool IncludeWwwroot { get; set; }
            public bool OverwriteExisting { get; set; } = true;
        }

        public class FtpToggleRequest : FtpRequestBase
        {
            public bool EnableOffline { get; set; }
        }

        public async Task<IActionResult> OnPostFtpToggleOfflineAsync([FromForm] FtpToggleRequest request)
        {
            if (!TryValidateFtpRequest(request, out var error))
            {
                return new JsonResult(new { success = false, message = error }) { StatusCode = 400 };
            }

            try
            {
                if (request.EnableOffline)
                {
                    var offlineContent = $"<html><body><h1>Server is being updated</h1><p>{DateTimeOffset.UtcNow:O}</p></body></html>";
                    await UploadTextAsync(request, CombineRemotePath(request.RemoteRoot, "app_offline.htm"), offlineContent);
                    return new JsonResult(new { success = true, message = "Web byl zastaven (app_offline.htm vytvo�en)." });
                }

                var deleted = await TryDeleteFileAsync(request, CombineRemotePath(request.RemoteRoot, "app_offline.htm"));
                return new JsonResult(new
                {
                    success = true,
                    message = deleted ? "Web byl spu�t�n (app_offline.htm odstran�n)." : "Web byl spu�t�n, ale app_offline.htm ne�lo odstranit."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Operace selhala: {ex.Message}" }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> OnPostFtpDeleteWebAsync([FromForm] FtpDeleteRequest request)
        {
            if (!TryValidateFtpRequest(request, out var error))
            {
                return new JsonResult(new { success = false, message = error }) { StatusCode = 400 };
            }

            try
            {
                await DeleteRemoteTreeAsync(request, request.RemoteRoot, request.KeepWwwroot);
                return new JsonResult(new { success = true, message = "Smaz�n� webu dokon�eno." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Smaz�n� webu selhalo: {ex.Message}" }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> OnPostFtpUploadFullAsync([FromForm] FtpUploadRequest request)
        {
            if (!TryValidateFtpRequest(request, out var error))
            {
                return new JsonResult(new { success = false, message = error }) { StatusCode = 400 };
            }

            var releasePath = string.IsNullOrWhiteSpace(request.LocalReleasePath)
                ? GetDefaultReleasePath()
                : request.LocalReleasePath.Trim();

            if (string.IsNullOrWhiteSpace(releasePath) || !Directory.Exists(releasePath))
            {
                return new JsonResult(new { success = false, message = "Cesta k release slo�ce neexistuje." }) { StatusCode = 400 };
            }

            try
            {
                await UploadDirectoryAsync(request, releasePath, request.RemoteRoot, request.IncludeWwwroot, request.OverwriteExisting);
                return new JsonResult(new { success = true, message = "Upload webu dokon�en." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Upload webu selhal: {ex.Message}" }) { StatusCode = 500 };
            }
        }


        
    }
}
