using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace <%= namespace %>.Tests
{
    public class AuthTokenHelper
    {
        private JwtConfig _jwtOptions;

        private const string SecretKey = "mydeepestdarkestsecretkey";

        public static SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(SecretKey));

        public AuthTokenHelper()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var configuration = configurationBuilder.Build();

            var jwtConfig = new JwtConfig();

            configuration.GetSection("JWTSettings").Bind(jwtConfig);

            _jwtOptions = jwtConfig;
        }


        private long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() -
                                 new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);


        public List<Claim> GetUserClaims(string[] groups = null)
        {
            var faker = new Faker();
            var userName = faker.Internet.UserName();
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();

            var groupClaims = groups.Select(group => new Claim(ClaimTypes.Groups, group)).ToList();

            var identity = new ClaimsIdentity(
                new GenericIdentity(userName, "Token"),
                groupClaims
            );

            var claims = new List<Claim>
            {
                new Claim("user_id", Guid.NewGuid().ToString()),
                new Claim("name", firstName + lastName),
                new Claim(JwtRegisteredClaimNames.Email, faker.Internet.Email(firstName, lastName)),
                new Claim(JwtRegisteredClaimNames.Sub, faker.Internet.UserName(firstName, lastName)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)
            };
            claims.AddRange(identity.FindAll(p => true));

            return claims;
        }

        public ApplicationUser GetUserToken(DateTime? expiration = null, string[] groups = null)
        {
            var claims = GetUserClaims(groups);

            if (!expiration.HasValue)
                expiration = DateTime.UtcNow.AddDays(1);

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.OrganizationUrl,
                audience: _jwtOptions.ClientId,
                claims: claims.ToArray(),
                notBefore: DateTime.UtcNow,
                expires: expiration,
                signingCredentials: new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256));

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            var user = jwt.GetUser();
            user.IdToken = token;
            
            return user;
        }

        public static ActionContext GetActionContext(params Claim[] claims)
        {
            var authData = new AuthTokenHelper();

            var groups = claims.Where(c => c.Type == ClaimTypes.Groups).Select(c => c.Value).ToArray();

            var headerDictionary = new HeaderDictionary
            {
                { "Authorization", $"Bearer {authData.GetUserToken(null, groups)}" }
            };

            var response = new Mock<HttpResponse>();
            response.SetupGet(r => r.Headers).Returns(headerDictionary);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authData.GetUserClaims(groups)));

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(a => a.Response).Returns(response.Object);
            httpContext.Setup(a => a.User).Returns(claimsPrincipal);


            var serviceCollectionMock = new Mock<IServiceCollection>();

            var services = new ServiceCollection().AddOptions()
                .AddAuthenticationCore(o =>
                {
                    o.DefaultScheme = "simple";
                    o.AddScheme("simple", s => s.HandlerType = typeof(FakeAuthenticationHandler));
                });
            httpContext.SetupGet(a => a.RequestServices).Returns(services.BuildServiceProvider());

            var actionContext = new ActionContext
            {
                HttpContext = httpContext.Object,
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            };

            return actionContext;
        }
    }
}