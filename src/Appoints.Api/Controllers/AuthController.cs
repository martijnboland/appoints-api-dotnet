using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using Appoints.Core.Data;
using Appoints.Core.Domain;
using Microsoft.Owin.Security;

namespace Appoints.Api.Controllers
{
    public class AuthController : ApiController
    {
        private readonly AppointsDbContext _dbContext;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public AuthController(AppointsDbContext dbContext, JwtSecurityTokenHandler tokenHandler)
        {
            _dbContext = dbContext;
            _tokenHandler = tokenHandler;
        }

        [Route("auth/{provider}")]
        [OverrideAuthentication]
        [AllowAnonymous]
        public IHttpActionResult GetExternalLogin(string provider)
        {
            var ctx = Request.GetOwinContext();
            var props = new AuthenticationProperties
                        {
                            RedirectUri =
                                VirtualPathUtility.ToAbsolute(
                                    string.Format("~/auth/{0}/callback", provider))
                        };
            ctx.Authentication.Challenge(props, provider);
            return StatusCode(HttpStatusCode.Unauthorized);
        }

        [Route("auth/{provider}/callback")]
        [OverrideAuthentication]
        [HostAuthentication("ExternalCookie")]
        public IHttpActionResult GetExternalLoginCallback(string provider, string error = null)
        {
            var externalUserIdentity = User.Identity as ClaimsIdentity;
            if (externalUserIdentity == null)
            {
                return Unauthorized();
            }
            var providerUserId = externalUserIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var dbUser =
                _dbContext.Users.Where(u => u.Provider == provider && u.ProviderUserId == providerUserId)
                    .Include(u => u.UserRoles.Select(ur => ur.Role))
                    .SingleOrDefault();
            if (dbUser == null)
            {
                var customerRole = _dbContext.Roles.SingleOrDefault(r => r.Name == RoleNames.Customer);
                dbUser = CreateNewUserFromIdentity(externalUserIdentity);
                if (customerRole != null)
                {
                    dbUser.UserRoles.Add(new UserRole {User = dbUser, Role = customerRole});
                }
                _dbContext.Users.Add(dbUser);
            }

            var issuer = ConfigurationManager.AppSettings["jwt:Issuer"];
            var audience = ConfigurationManager.AppSettings["jwt:Audience"];
            var secret = ConfigurationManager.AppSettings["jwt:SecretKey"];

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, dbUser.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, dbUser.DisplayName));
            claims.Add(new Claim(ClaimTypes.Email, dbUser.Email));
            claims.AddRange(dbUser.UserRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole.Role.Name)));

            var jwtToken = new JwtSecurityToken(issuer, audience, claims, null,
                DateTime.UtcNow.AddMinutes(JwtSecurityTokenHandler.DefaultTokenLifetimeInMinutes),
                new SigningCredentials(new InMemorySymmetricSecurityKey(Convert.FromBase64String(secret)),
                    SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest));
            var token = _tokenHandler.WriteToken(jwtToken);
            
            dbUser.LastAuthenticated = DateTime.UtcNow;
            _dbContext.SaveChanges();

            var redirectUrl = Request.RequestUri.OriginalString.Replace(provider + "/callback", "loggedin") +
                              "#access_token=" + token;
            return Redirect(redirectUrl);
        }

        [Route("auth/loggedin")]
        [OverrideAuthentication]
        [AllowAnonymous]
        public HttpResponseMessage GetLoggedIn()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            var contentString = @"
<html>
  <head>
    <script>
      if (window.opener) { window.opener.postMessage(window.location.hash.replace('#access_token=', ''), '*'); }
    </script>
  </head>
  <body>
  </body>
</html>";
            response.Content = new StringContent(contentString, Encoding.UTF8, "text/html");
            return response;
        }

        public static User CreateNewUserFromIdentity(ClaimsIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            var providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

            if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                || String.IsNullOrEmpty(providerKeyClaim.Value))
            {
                return null;
            }

            if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
            {
                return null;
            }

            var nameClaim = identity.FindFirst(ClaimTypes.Name);
            var emailClaim = identity.FindFirst(ClaimTypes.Email);

            return new User
                   {
                       ProviderUserId = providerKeyClaim.Value,
                       Provider = providerKeyClaim.Issuer,
                       Email = emailClaim != null ? emailClaim.Value : null,
                       DisplayName = nameClaim != null ? nameClaim.Value : null,
                       Created = DateTime.UtcNow
                   };
        }
    }
}