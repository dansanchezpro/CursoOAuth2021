using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherApi.Policies
{
    public class ScopeHandler : AuthorizationHandler<ScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type ==
             "http://schemas.microsoft.com/identity/claims/scope" &&
             c.Issuer == requirement.Issuer))
            {
                string[] Scopes = context.User.FindFirst(c => c.Type == "http://schemas.microsoft.com/identity/claims/scope" &&
                c.Issuer == requirement.Issuer).Value.Split(' ');
                if (Scopes.Any(s => s == requirement.Scope))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
