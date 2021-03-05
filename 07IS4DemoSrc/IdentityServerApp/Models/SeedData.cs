using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerApp.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
namespace IdentityServerApp.Models
{
    public static class SeedData
    {
        public static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var ServiceScope =
            app.ApplicationServices
            .GetService<IServiceScopeFactory>().CreateScope())
            {
                // Aplicar migración de PersistedGrantDbContext.
                // Crea la base de datos si no existe.
                ServiceScope
                .ServiceProvider
                .GetRequiredService<PersistedGrantDbContext>()
                .Database.Migrate();
                // Obtener el contexto ConfigurationDbContext.
                var Context = ServiceScope
                .ServiceProvider
                .GetRequiredService<ConfigurationDbContext>();
                // Aplicar migración de ConfigurationDbContext.
                // Crea la base de datos si no existe.
                Context.Database.Migrate();
                // El siguiente código nos evita registrar manualmente
                // los datos de Config.cs a la Base de datos.
                // Copiar Config.ApiScopes a la base de datos.
                if (!Context.ApiScopes.Any())
                {
                    Context.ApiScopes.AddRange(
                    Config.ApiScopes
                    .Select(a => a.ToEntity()).ToList());
                    Context.SaveChanges();
                }
                // Copiar Config.ApiResources a la base de datos.
                if (!Context.ApiResources.Any())
                {
                    Context.ApiResources.AddRange(
                    Config.ApiResources
                    .Select(a => a.ToEntity()).ToList());
                    Context.SaveChanges();
                }
                // Copiar Config.Clients a la base de datos
                if (!Context.Clients.Any())
                {
                    Context.Clients.AddRange(
                    Config.Clients
                    .Select(c => c.ToEntity()).ToList());
                    Context.SaveChanges();
                }
                // Copiar Config.IdentityResources a la base de datos
                if (!Context.IdentityResources.Any())
                {
                    Context.IdentityResources.AddRange(
                    Config.IdentityResources
                    .Select(i => i.ToEntity()).ToList());
                    Context.SaveChanges();
                }
            }
        }
    }
}
