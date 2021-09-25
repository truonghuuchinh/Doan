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
                    options.User.RequireUniqueEmail = true;
                });
                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Administration/Home/Login";
                    options.AccessDeniedPath = "/Administration/Home/Login";
                 
                });
                services.AddDistributedMemoryCache();

                services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });
            });
        }
    }
}