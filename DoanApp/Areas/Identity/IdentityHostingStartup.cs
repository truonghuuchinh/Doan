using System;
using DoanData.DoanContext;
using DoanData.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(DoanApp.Areas.Identity.IdentityHostingStartup))]
namespace DoanApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddIdentity<AppUser,AppRole>()
              .AddEntityFrameworkStores<DpContext>().AddDefaultTokenProviders();
                services.Configure<IdentityOptions>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.Password.RequireUppercase = false;
                    
                   
                });
                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Home/Login";
                    options.AccessDeniedPath = "/Administration/Home/Login";
                });
            });
        }
    }
}