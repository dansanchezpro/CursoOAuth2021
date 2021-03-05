using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IdentityServerApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=ISCourse";

            var MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //var Builder = services.AddIdentityServer()
            //    .AddInMemoryApiScopes(Identity.Config.ApiScopes)
            //    .AddInMemoryApiResources(Identity.Config.ApiResources)
            //    .AddInMemoryClients(Identity.Config.Clients)
            //    .AddTestUsers(Models.Repository.Users)
            //    .AddInMemoryIdentityResources(Identity.Config.IdentityResources);
            var Builder = services.AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(ConnectionString, sql =>
                    sql.MigrationsAssembly(MigrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(ConnectionString, sql =>
                    sql.MigrationsAssembly(MigrationsAssembly));
                })
                .AddTestUsers(Models.Repository.Users);


            Builder.AddDeveloperSigningCredential();
            services.AddControllersWithViews();
            services.AddAuthentication()
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options => 
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "893340720053-mu3fihlubgpi5ad6fdbih2dtu2sbfaom.apps.googleusercontent.com";
                    options.ClientSecret = "C4yE0tvK3QsAzLHm0P9mihs2";
                })
                .AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "2855936137964511";
                    options.ClientSecret = "51bd55c31174fd36818e85c1de936cb5";
                })
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Models.SeedData.InitializeDatabase(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name:"default",
                    pattern:"{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
