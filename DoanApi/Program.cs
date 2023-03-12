using System.Collections.Generic;
using DoanApp.Services;
using DoanApp.ValidatorModel;
using DoanData.DoanContext;
using DoanData.Models;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;

builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<DpContext>().
                AddDefaultTokenProviders();
builder.Services.AddDbContext<DpContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DpContext"));
}, ServiceLifetime.Transient);

builder.Services.AddLogging();
builder.Services.AddTransient<IVideoService, VideoService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IReportVideoService, ReportVideoService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IMessageService, MessageService>();
builder.Services.AddTransient<ICommentService, CommentService>();
//Register Fluent for all validator
builder.Services.AddControllers().AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<LoginValidator>());

builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "Project Api", Version = "v1" });
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = string.Format("JWT Authoration header using  the Bearer scheme. \r\n\r\nEnter 'Bearer' [space] and then your token in text input below \r\n\r\nExample:'Bearer 123456ADEF'"),
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    x.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
 .AddJwtBearer(option =>
 {
     option.RequireHttpsMetadata = false;
     option.SaveToken = true;
     option.TokenValidationParameters = new TokenValidationParameters()
     {
         ValidateIssuer = true,
         ValidIssuer = configuration.GetValue<string>("Tokens:Issuer"),
         ValidateAudience = true,
         ValidAudience = configuration.GetValue<string>("Tokens:Issuer"),
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ClockSkew = System.TimeSpan.Zero,
         IssuerSigningKey = new SymmetricSecurityKey(
             System.Text.Encoding.UTF8.GetBytes(configuration.GetValue<string>("Tokens:Key"))
        )
     };
 });

var app = builder.Build();

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

app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Doan v1");
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();