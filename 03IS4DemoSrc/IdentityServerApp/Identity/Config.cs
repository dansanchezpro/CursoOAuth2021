﻿using IdentityServer4;
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

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address()
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
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials
                },
                new Client
                {
                    ClientId = "MVCClientApp",
                    ClientSecrets =
                    {
                        new Secret("Secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        "weatherapi.read", "weatherapi.write"
                    },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = 
                    {
                        "https://localhost:44302/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44302/signout-callback-oidc"
                    },
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            };

    }
}
