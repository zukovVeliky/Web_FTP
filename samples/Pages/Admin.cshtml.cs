using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Samples.Pages;

public class AdminModel : PageModel
{
    public FTP.FTP_UpdateModel fTP_Update;

    public AdminModel(IConfiguration con)
    {
        fTP_Update = new FTP.FTP_UpdateModel(con);
    }

    public async Task OnGetAsync()
    {
        await fTP_Update.OnGetAsync();
    }

    #region FTP

    public async Task<IActionResult> OnPostFtpDeployAsync([FromForm] FTP.FTP_UpdateModel.FtpDeployRequest request)
    {
        var result = await fTP_Update.OnPostFtpDeployAsync(request);
        return result;
    }

    public async Task<IActionResult> OnPostFtpReadConfigAsync([FromForm] FTP.FTP_UpdateModel.FtpConfigRequest request)
    {
        var result = await fTP_Update.OnPostFtpReadConfigAsync(request);
        return result;
    }

    public async Task<IActionResult> OnPostFtpWriteConfigAsync([FromForm] FTP.FTP_UpdateModel.FtpConfigRequest request)
    {
        var result = await fTP_Update.OnPostFtpWriteConfigAsync(request);
        return result;
    }

    public async Task<IActionResult> OnPostFtpToggleOfflineAsync([FromForm] FTP.FTP_UpdateModel.FtpToggleRequest request)
    {
        var result = await fTP_Update.OnPostFtpToggleOfflineAsync(request);
        return result;
    }

    public async Task<IActionResult> OnPostFtpDeleteWebAsync([FromForm] FTP.FTP_UpdateModel.FtpDeleteRequest request)
    {
        var result = await fTP_Update.OnPostFtpDeleteWebAsync(request);
        return result;
    }

    public async Task<IActionResult> OnPostFtpUploadFullAsync([FromForm] FTP.FTP_UpdateModel.FtpUploadRequest request)
    {
        var result = await fTP_Update.OnPostFtpUploadFullAsync(request);
        return result;
    }

    #endregion
}
