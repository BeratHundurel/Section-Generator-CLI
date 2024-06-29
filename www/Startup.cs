using entity.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using service.Abstract;
using service.Concrete.EntityFramework;
using System;
using System.Globalization;

namespace www
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => { options.LoginPath = "/Authentication/LoginCustomer"; });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
            services.AddDetection();

            if (Environment.IsDevelopment())
            {
                services.AddControllersWithViews(x => x.SuppressAsyncSuffixInActionNames = false).AddRazorRuntimeCompilation();


                services.AddDbContext<DatabaseContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("Development")));

            }
            else
            {
                services.AddControllersWithViews();

                services.AddDbContext<DatabaseContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("Production")));
            }

            //This provide to solve uow service.
            services.AddTransient<IUnitOfWork, EfUnitOfWork>();
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
            app.UseDetection();
            //app.UseStaticFiles(new StaticFileOptions()
            //         {
            //             HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
            //             OnPrepareResponse = (context) =>
            //             {
            //                 var headers = context.Context.Response.GetTypedHeaders();
            //                 headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            //                 {
            //                     Public = true,
            //                     MaxAge = TimeSpan.FromDays(30)
            //                 };
            //             }
            //         });

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Cache static files for 30 days
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");
                    ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(30).ToString("R", CultureInfo.InvariantCulture));
                }
            });

            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "default",
                template: "{controller=Main}/{action=Index}/{id?}");

                routes.MapRoute(
                   name: "permalink",
                   template: "/{permaLink?}",
                   defaults: new { controller = "Main", action = "Index" });

            });
        }
    }
}
