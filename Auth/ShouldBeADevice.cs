using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemindONServer.Auth
{
    public class ShouldBeADeviceRequirement
     : IAuthorizationRequirement
        {
        }

    public class ShouldBeADeviceRequirementHandler
    : AuthorizationHandler<ShouldBeADeviceRequirement>
    {

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ShouldBeADeviceRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == ClaimTypes.Role))
                return Task.CompletedTask;

            var claim = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

            var role = claim.Value;

            if (role == Roles.Device)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
