using System.IO;
using ASPNETCoreLvl2Demo.HealthChecks;
using ASPNETCoreLvl2Demo.Identity;
using ASPNETCoreLvl2Demo.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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

            services.AddIdentityCore<CustomUser>();
            services.AddSingleton<IUserStore<CustomUser>, CustomUserStore>();
            services.AddSingleton<IAuthorizationHandler, VaccinationRequirementHandler>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o => o.LoginPath = "/Account/Login");

            services.AddAuthorization(o =>
            {
                o.AddPolicy("HasFavoriteMusician", p =>
                    p.RequireClaim("FavoriteMusician"));
                o.AddPolicy("IsVaccinated", p => p.Requirements.Add(new VaccinationRequirement("Sputnik-V")));
                o.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

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