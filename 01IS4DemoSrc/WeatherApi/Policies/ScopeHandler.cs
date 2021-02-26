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
            const string MicrosoftScopeClaim = "http://schemas.microsoft.com/identity/claims/scope";
            var ScopeClaim = context.User.FindFirst(c => c.Type == MicrosoftScopeClaim && c.Issuer == requirement.Issuer);

            if (ScopeClaim != null)
            {
                string[] Scopes = ScopeClaim.Value.Split(' ');
                if (Scopes.Any(s => s == requirement.Scope))
                {
                    context.Succeed(requirement);
                }
            }
            else if(context.User.HasClaim(c=> c.Type == "scope" &&
            c.Value == requirement.Scope && c.Issuer == requirement.Issuer))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }
    }
}
