using AjcProject.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AjcProject
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
            services.AddSingleton<IDateTime, SystemDateTime>();
            services.AddSingleton<IRandomCode, RandomCode>();
            services.AddDbContext<AjcProjectDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:SqlDbConnection"], sqlServerOptions =>
                { });
            });

            services.Configure<FormOptions>(options =>
            {
                // Set the limit to 128 MB =  134217728
                options.MultipartBodyLengthLimit = 2097152;
            });

            services.AddAuthentication()
.AddGoogle(options =>
{
    options.ClientId = "960435897222-iofoun7a4r64qevnfupcdenleco0jqr6.apps.googleusercontent.com";
    options.ClientSecret = "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";
});


            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var cultures = new List<CultureInfo> { new CultureInfo("fr-FR") };
            app.UseRequestLocalization(options => {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("fr-FR");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

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

            //app.UseSession();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

        }
    }
}
