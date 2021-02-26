using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerApp.Identity
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("weatherapi.read", "Read Weather API"),
                new ApiScope("weatherapi.write", "Write Weather API")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("api://weatherapi", "Weather API")
                {
                    Scopes = { "weatherapi.read", "weatherapi.write" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "ClientApp",
                    ClientSecrets =
                    {
                        new Secret("Secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        "weatherapi.read", "weatherapi.write"
                    },
                    AllowedGrantTypes = GrantTypes.ClientCredentials
                }
            };

    }
}
