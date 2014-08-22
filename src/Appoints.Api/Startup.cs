using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using Appoints.Core.Data;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Jwt;
using Owin;

[assembly: OwinStartup(typeof (Appoints.Api.Startup))]

namespace Appoints.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // AutoFac config
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.Register(c => new AppointsDbContext("AppointsDb"));
            builder.Register(c => new JwtSecurityTokenHandler());
            var container = builder.Build();

            var config = new HttpConfiguration();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            // CORS
            var allowedOriginsString = ConfigurationManager.AppSettings["cors:AllowedOrigins"];
            if (! String.IsNullOrEmpty(allowedOriginsString))
            {
                var corsPolicy = new CorsPolicy
                {
                    AllowAnyHeader = true,
                    AllowAnyMethod = true,
                    SupportsCredentials = true
                };
                var allowedOrigins = allowedOriginsString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var allowedOrigin in allowedOrigins)
                {
                    corsPolicy.Origins.Add(allowedOrigin);
                }
                app.UseCors(new CorsOptions
                {
                    PolicyProvider = new CorsPolicyProvider
                    {
                        PolicyResolver = context => Task.FromResult(corsPolicy)
                    }
                });                
            }

            // Authentication/Authorization
            var issuer = ConfigurationManager.AppSettings["jwt:Issuer"];
            var audience = ConfigurationManager.AppSettings["jwt:Audience"];
            var secret = ConfigurationManager.AppSettings["jwt:SecretKey"];

            // Default JWT token based authorization
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] {audience},
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                                                   {
                                                       new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                                                   }
                });

            // Required for 3rd-party login flow.
            var externalCookie = new CookieAuthenticationOptions
                                 {
                                     AuthenticationType = "ExternalCookie",
                                     AuthenticationMode = AuthenticationMode.Passive
                                 };
            app.UseCookieAuthentication(externalCookie);

            app.UseFacebookAuthentication(new FacebookAuthenticationOptions
                                          {
                                              AppId = ConfigurationManager.AppSettings["facebook:AppId"],
                                              AppSecret = ConfigurationManager.AppSettings["facebook:AppSecret"],
                                              AuthenticationType = "facebook",
                                              AuthenticationMode = AuthenticationMode.Passive,
                                              SignInAsAuthenticationType = "ExternalCookie"
                                          });
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
                                        {
                                            ClientId = ConfigurationManager.AppSettings["google:ClientId"],
                                            ClientSecret = ConfigurationManager.AppSettings["google:ClientSecret"],
                                            AuthenticationType = "google",
                                            AuthenticationMode = AuthenticationMode.Passive,
                                            SignInAsAuthenticationType = "ExternalCookie"
                                        });

            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}