using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace <%= namespace %>
{
    /// <summary>
    /// Authentication extension methods
    /// </summary>
    public static class AuthExtensions
    {
        /// <summary>
        /// Extension method for creating a ApplicationUser from the ClaimsPrincipal.
        /// </summary>
        /// <param name="context">The http context.</param>
        public static ApplicationUser GetUser(this HttpContext context)
        {
            if (context.User != null)
            {
                var userId = context.User.FindFirst("user_id");
                var email = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email);
                var name = context.User.FindFirst("name");
                var token = context.GetTokenAsync("access_token").Result;

                var user = new ApplicationUser
                {
                    UserId = userId?.Value,
                    Email = email?.Value,
                    DisplayName = name?.Value,
                    IdToken = token,
                    Groups = context.User.Claims.Where(x => x.Type == ClaimTypes.Groups).Select(s => s.Value).ToList()
                };

                return user;
            }
            return null;
        }

        /// <summary>
        /// Extension method for creating a ApplicationUser from JwtSecurityToken.
        /// </summary>
        /// <param name="value">The JwtSecurityToken.</param>
        public static ApplicationUser GetUser(this JwtSecurityToken value)
        {
            if (value == null)
                return null;


            var userId = value.Claims.FirstOrDefault(x => x.Type == "user_id");
            var email = value.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email);
            var name = value.Claims.SingleOrDefault(x => x.Type == "name");

            return new ApplicationUser
            {
                UserId = userId?.Value,
                Email = email?.Value,
                DisplayName = name?.Value,
                IdToken = value.RawData,
                Groups = value.Claims.Where(x => x.Type == ClaimTypes.Groups).Select(s => s.Value).ToList()
            };
        }

        /// <summary>
        /// Validates if the given user belongs to a certain group.
        /// </summary>
        /// <param name="principal">The users claim principal.</param>
        /// <param name="group">The group to check.</param>
        public static bool HasGroup(this ClaimsPrincipal principal, string group) => principal.HasClaim(ClaimTypes.Groups, group);
    }
}