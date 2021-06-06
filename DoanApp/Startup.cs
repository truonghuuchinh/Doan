using DoanApp.Services;
using DoanData.DoanContext;
using DoanData.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoanApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            IMvcBuilder builder = services.AddRazorPages();
            builder.AddRazorRuntimeCompilation();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDbContext<DpContext>(options=>options.UseSqlServer(Configuration.GetConnectionString("DpContext")), ServiceLifetime.Transient);
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IVideoService, VideoService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<ILikeVideoService, LikeVideoService>();
            services.AddTransient<ILikeCommentService, LikeCommentService>();
            services.AddTransient<IFollowChannelService, FollowChannelService>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddAuthentication()
                 .AddFacebook(options =>
                 {
                     options.AppSecret = "bd86615d96bda050d19536a5fce84e44";
                     options.AppId = "794564367817812";
                     options.Fields.Add("picture");
                     options.Fields.Add("birthday");
                     options.Events = new OAuthEvents
                     {
                         OnCreatingTicket = context =>
                         {
                             var identity = (ClaimsIdentity)context.Principal.Identity;
                             var profileImg = context.User.GetProperty("picture").GetProperty("data").GetProperty("url").ToString();
                             var birthDay = context.User.GetProperty("birthday").ToString();
                             identity.AddClaim(new Claim(JwtClaimTypes.Picture, profileImg));
                             identity.AddClaim(new Claim(JwtClaimTypes.BirthDate, birthDay));
                             return Task.CompletedTask;
                         }
                     };
                 })
                 .AddGoogle(options =>
                 {
                     options.ClientSecret = "T18avtximxpvbzvAcPUVpD3-";
                     options.ClientId = "226619273128-6phcmqe72n6h8rlson2brp905gpm0cc5.apps.googleusercontent.com";
                     options.Scope.Add("profile");

                     options.Events.OnCreatingTicket = (context) =>
                     {
                         var picture = context.User.GetProperty("picture").GetString();
                         context.Identity.AddClaim(new Claim("picture", picture));
                         return Task.CompletedTask;
                     };
                 })
                 ;
            
        }
        [Obsolete]
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                  name: "Administration",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Home}/{action=Index}/{id?}");
               
            });
        }
    }
}
