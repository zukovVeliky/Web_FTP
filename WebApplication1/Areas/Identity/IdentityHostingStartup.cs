using System;
using Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Identity.IdentityHostingStartup))]
namespace Identity
{
    
    public class IdentityHostingStartup : IHostingStartup
    {
        
        public void Configure(IWebHostBuilder builder)
        {

            builder.ConfigureServices((context, services) =>
            {
                var connect = context.Configuration.GetConnectionString("IdentityConnection");

                services.AddDbContext<Identity.DB_Context.IdentityContext>(options => options.UseSqlServer(connect));
                services.AddDbContext<Identity.DB_Context.IdentityContextDB_Set>(options => options.UseSqlServer(connect));

                services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<Identity.DB_Context.IdentityContextDB_Set>().AddDefaultTokenProviders();

                // registrovat emailovou slu?bu v metod? ConfigureServices
                services.Configure<EmailSettings>(context.Configuration.GetSection("EmailSettings"));
                services.AddSingleton<IEmailSender, EmailSender>();
                
            });

        }
    }
}