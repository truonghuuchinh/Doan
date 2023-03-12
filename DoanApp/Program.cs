using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DoanApp.Hubs;
using DoanApp.Models;
using DoanApp.ServiceApi;
using DoanApp.Services;
using DoanApp.ValidatorModel;
using DoanData.DoanContext;
using DoanData.Models;
using FluentValidation.AspNetCore;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;

// Config async view when edit
#if DEBUG 
IMvcBuilder builderMvc = builder.Services.AddRazorPages();
if (env.IsDevelopment())
    builderMvc.AddRazorRuntimeCompilation();
#endif

// Add http client to call api
builder.Services.AddHttpClient();

// Register Fluent for all validator
builder.Services.AddControllersWithViews()
.AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<LoginValidator>());
builder.Services.AddDistributedMemoryCache();

// Config Sesssion
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add connection String
builder.Services.AddDbContext<DpContext>(options => options.
UseSqlServer(configuration.GetConnectionString("DpContext")), ServiceLifetime.Transient);

// Add Service Idetity 
builder.Services.AddIdentity<AppUser, AppRole>()
 .AddEntityFrameworkStores<DpContext>().AddDefaultTokenProviders();

// Add options for email setting
builder.Services.Configure<EmailSettings>(option => builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient(option => option.GetRequiredService<EmailSettings>());

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Administration/Home/Login";
    options.AccessDeniedPath = "/Administration/Home/Login";

});

// Add service Controller
builder.Services.AddTransient<IMessageService, MessageService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IVideoService, VideoService>();
builder.Services.AddTransient<ICommentService, CommentService>();
builder.Services.AddTransient<ILikeVideoService, LikeVideoService>();
builder.Services.AddTransient<ILikeCommentService, LikeCommentService>();
builder.Services.AddTransient<IFollowChannelService, FollowChannelService>();
builder.Services.AddTransient<IPlayListService, PlayListService>();
builder.Services.AddTransient<IDetailVideoService, DetailVideoService>();
builder.Services.AddTransient<IReportVideoService, ReportVideoService>();
builder.Services.AddTransient<IVideoWatchedService, VideoWatchedService>();
builder.Services.AddTransient<IUserApiCient, UserApiClient>();
builder.Services.AddTransient<IReportApiClient, ReportApiClient>();
builder.Services.AddTransient<ICategoryApiClient, CategoryApiClient>();

// Config to login with external api such as: facebook, google..., something like that
builder.Services.AddAuthentication()
     .AddFacebook(options =>
     {
         options.AppSecret = "bd86615d96bda050d19536a5fce84e44";
         options.AppId = "794564367817812";
         options.Fields.Add("picture");
         options.Fields.Add("email");
         options.Events = new OAuthEvents
         {
             OnCreatingTicket = context =>
             {
                 var identity = (ClaimsIdentity)context.Principal.Identity;
                 var profileImg = context.User.GetProperty("picture").GetProperty("data").GetProperty("url").ToString();
                 identity.AddClaim(new Claim(JwtClaimTypes.Picture, profileImg));
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
     });
builder.Services.AddSignalR();

// Build app
var app = builder.Build();


if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https:// aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();
app.UseSession();

// Add signalR to chat real time 
app.MapHub<ChatHub>("/MessageChat");

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();

    // Route Area for Admin
    endpoints.MapControllerRoute(
        name: "admin",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    // Route User
    endpoints.MapControllerRoute(
        name: "user",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();