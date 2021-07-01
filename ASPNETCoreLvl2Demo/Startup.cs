using System;
using System.IO;
using ASPNETCoreLvl2Demo.HealthChecks;
using ASPNETCoreLvl2Demo.Identity;
using ASPNETCoreLvl2Demo.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace ASPNETCoreLvl2Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddIdentityCore<CustomUser>(o =>
                {
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = true;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequiredLength = 5;
                })
                .AddUserStore<CustomUserStore>()
                .AddSignInManager<SignInManager<CustomUser>>()
                .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();
            services.Configure<DataProtectionTokenProviderOptions>(o =>
                o.TokenLifespan = TimeSpan.FromHours(3));
            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme, o => o.LoginPath = "/Account/Login");

            services.AddAuthorization(o =>
            {
                var musicianPolicy = new AuthorizationPolicyBuilder().RequireClaim("FavoriteMusician").Build();
                var vaccinatedPolicy = new AuthorizationPolicyBuilder().AddRequirements(new VaccinationRequirement("Sputnik-V")).Build();
                o.AddPolicy("HasFavoriteMusician", musicianPolicy);
                o.AddPolicy("IsVaccinated", vaccinatedPolicy);
                o.AddPolicy("CombinedPolicy", AuthorizationPolicy.Combine(musicianPolicy, vaccinatedPolicy));
                o.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
            services.AddSingleton<IAuthorizationHandler, VaccinationRequirementHandler>();


            services.AddDirectoryBrowser();

            services.AddHealthChecks()
                .AddCheck<SomeHealthCheck>("some_health_check");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // using Microsoft.AspNetCore.Http;
                    ctx.Context.Response.Headers.Append(
                        "Cache-Control", $"public, max-age=60000");
                }
            });
            
            app.UseRouting();

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.WebRootPath, "images")),
                RequestPath = "/MyImages"
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "StaticFiles")),
                RequestPath = "/StaticFiles"
            });

            app.UseEndpointsLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    AllowCachingResponses = false
                }).RequireAuthorization("IsVaccinated");
            });
        }
    }
}