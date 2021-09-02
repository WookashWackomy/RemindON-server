using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using RemindONServer.Domain.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using RemindONServer.Controllers.Utils;
using RemindONServer.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using RemindONServer.Middlewares;
using System.Reflection;
using System.IO;
using RemindONServer.Domain.Repositories;
using RemindONServer.Domain.Services;
using RemindONServer.Domain.Persistence.Contexts;
using RemindONServer.Persistence.Repositories;

namespace RemindONServer
{
    public class Startup
    {
        readonly string AllowSpecificOriginsPolicy = "_AllowSpecificOriginsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DB_MAIN")));
            services.AddDefaultIdentity<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;

                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            })
             .AddEntityFrameworkStores<ApplicationDbContext>();


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.Name = "AuthCookie";
                options.Events.OnRedirectToAccessDenied = ResponseHelpers.UnAuthorizedResponse;
                options.Events.OnRedirectToLogin = ResponseHelpers.UnAuthorizedResponse;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = new TimeSpan(1, 0, 0);
            })
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", options => { });

            services.AddAuthorization(config =>
                {
                    config.AddPolicy("ShouldBeAnUser", options =>
                    {
                        options.RequireAuthenticatedUser();
                        options.AuthenticationSchemes.Add(
                                CookieAuthenticationDefaults.AuthenticationScheme);
                        options.Requirements.Add(new ShouldBeAnUserRequirement());
                    });

                    config.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build());
                });

            services.AddCors(o => o.AddPolicy(AllowSpecificOriginsPolicy,
                      builder =>
                        builder//.WithOrigins("http://localhost:3000", "https://localhost:3000", "https://dev01-remindon.netlify.app", "http://dev01-remindon.netlify.app")
                      .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials()
                       .SetIsOriginAllowed(_ => true)
                      ));

            services.AddScoped<IAuthorizationHandler, ShouldBeAnUserRequirementHandler>();

            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<IPrescriptionsService, PrescriptionService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0.1", new OpenApiInfo
                {
                    Version = "v0.1",
                    Title = "RemindON API",
                    Description = "RemindON Web API",
                    TermsOfService = new Uri("https://remindonserverprod.azurewebsites.net/tos"),
                    Contact = new OpenApiContact
                    {
                        Name = "Łukasz Łakomy",
                        Email = "wookie.xp.07@gmail.com",
                        Url = new Uri("https://www.github.com/wookashwackomy"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = new Uri("https://example.com/license"), //TODO
                    }
                });
                

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseMiddleware<RequestDiagnosticsMiddleware>();
                app.UseDeveloperExceptionPage();
                app.UseRouteDebugger();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            var cookiePolicyOptions = new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.SameAsRequest,
                MinimumSameSitePolicy = SameSiteMode.None
            };

            app.UseCookiePolicy(cookiePolicyOptions);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(AllowSpecificOriginsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });




            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v0.1/swagger.json", "RemindON API V0.1");
            });
        }
    }
}
