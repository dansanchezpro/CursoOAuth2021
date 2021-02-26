using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherApi.Policies
{
    public class ScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; set; }
        public string Scope { get; set; }
        public ScopeRequirement(string scope, string issuer)
        {
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
    }
}
