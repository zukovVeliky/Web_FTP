using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FileManager
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class MiddlewareFileManager
    {
        private readonly RequestDelegate _next;

        public MiddlewareFileManager(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            Blog.Areas.FileManager.Models.fileUpload FU = new Blog.Areas.FileManager.Models.fileUpload(httpContext);

            if (!Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(httpContext.Request.Query["CKEUI"]))
            {
                httpContext.Response.WriteAsync("<script type='text/javascript'>window.parent.CKEDITOR.tools.callFunction(" + httpContext.Request.Query["CKEditorFuncNum"] + ", document.location.origin+'/UzivatelskeSoubory/" + FU.CkEditorFile + "');</script>");
            }
            else
            {

            }
            if (!Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(httpContext.Request.Query["CKEU"]))
            {
                httpContext.Response.WriteAsync("<script type='text/javascript'>window.parent.CKEDITOR.tools.callFunction(" + httpContext.Request.Query["CKEditorFuncNum"] + ", document.location.origin+'/UzivatelskeSoubory/" + FU.CkEditorFile + "');</script>");
            }
            else
            {

            }
            

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareFileManagerExtensions
    {
        public static IApplicationBuilder UseMiddlewareFileManager(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MiddlewareFileManager>();
        }
    }
}
