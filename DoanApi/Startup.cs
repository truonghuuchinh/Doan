using DoanApp.Models;
using DoanApp.Services;
using DoanApp.ValidatorModel;
using DoanData.DoanContext;
using DoanData.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApi
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
           
            services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<DpContext>().
                AddDefaultTokenProviders();
            services.AddDbContext<DpContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DpContext")), ServiceLifetime.Transient);
            services.AddTransient<IVideoService, VideoService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IReportVideoService, ReportVideoService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ICommentService, CommentService>();
            //Register Fluent for all validator
            services.AddControllers().AddFluentValidation(x=>x.RegisterValidatorsFromAssemblyContaining<LoginValidator>());
           
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "Project Api", Version = "v1" });
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authoration header using  the Bearer scheme."+
                        "\r\n\r\nEnter 'Bearer' [space] and then your token in text input below"+
                    "\r\n\r\nExample:'Bearer 123456ADEF'",
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
                           Reference=new OpenApiReference
                           {
                               Type=ReferenceType.SecurityScheme,
                               Id="Bearer"
                           },
                           Scheme="oauth2",
                           Name="Bearer",
                           In=ParameterLocation.Header,
                       },
                       new List<string>()
                    }
                });
            });
            string issuer = Configuration.GetValue<string>("Tokens:Issuer");
            string signingKey = Configuration.GetValue<string>("Tokens:Key");
            byte[] signingKeyByteKey = System.Text.Encoding.UTF8.GetBytes(signingKey);
            services.AddAuthentication(options =>
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
                     ValidIssuer = issuer,
                     ValidateAudience = true,
                     ValidAudience = issuer,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ClockSkew = System.TimeSpan.Zero,
                     IssuerSigningKey = new SymmetricSecurityKey(signingKeyByteKey)
                 };
             });


        }

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

            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json","Swagger Doan v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
