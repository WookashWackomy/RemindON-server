using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RemindONServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemindONServer.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ApplicationDbContext _dbContext;
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ApplicationDbContext dbContext
            )
    : base(options, logger, encoder, clock)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // skip authentication if endpoint has [AllowAnonymous] attribute
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return Task.FromResult(AuthenticateResult.NoResult());

            Response.Headers.Add("WWW-Authenticate", "Basic");

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header missing."));
            }

            // Get authorization key
            //var authorizationHeader = Request.Headers["Authorization"].ToString();
            //var authHeaderRegex = new Regex(@"Basic (.*)");

            //if (!authHeaderRegex.IsMatch(authorizationHeader))
            //{
            //    return Task.FromResult(AuthenticateResult.Fail("Authorization code not formatted properly."));
            //}

            try
            {
                //var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
                var authHeader = Request.Headers["Authorization"].ToString();
                //var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                // var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var credentials = authHeader.Split(new[] { ':' }, 2);
                var serialNumber = credentials[0];
                var password = credentials[1];


                RemindONDevice device = _dbContext.RemindONDevices.FirstOrDefault(d => d.SerialNumber == serialNumber);
                if (device == null)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
                }

                if (device.Password != password)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, device.SerialNumber),
                    new Claim(ClaimTypes.Name, device.SerialNumber),
                    new Claim (ClaimTypes.Role, Roles.StandardUser)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }

        }
    }
}
