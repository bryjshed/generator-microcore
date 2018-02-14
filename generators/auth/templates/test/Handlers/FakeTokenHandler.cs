using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace <%= namespace %>.Tests
{
    /// <summary>
    /// Token handler to disable token authentication since it will be done in the api gateway.
    /// </summary>
    public class FakeTokenHandler : JwtSecurityTokenHandler
    {
        /// <summary>
        /// Overrides the token validation to create a specific validation to disable Signature validation.
        /// </summary>
        /// <param name="token">The id token</param>
        /// <param name="validationParameters">The validation params which are ignored</param>
        /// <param name="validatedToken">The SecurityToken that is set.</param>
        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw LogHelper.LogArgumentNullException(nameof(token));
            }
            if (validationParameters == null)
            {
                throw LogHelper.LogArgumentNullException(nameof(validationParameters));
            }

            JwtSecurityToken jwt = ReadJwtToken(token);

            DateTime? expires = (jwt.Payload.Exp == null) ? null : new DateTime?(jwt.ValidTo);
            DateTime? notBefore = (jwt.Payload.Nbf == null) ? null : new DateTime?(jwt.ValidFrom);

            if (validationParameters.ValidateLifetime)
            {
                if (validationParameters.LifetimeValidator != null)
                {
                    if (!validationParameters.LifetimeValidator(notBefore: notBefore, expires: expires, securityToken: jwt, validationParameters: validationParameters))
                        throw LogHelper.LogExceptionMessage(new SecurityTokenInvalidLifetimeException(String.Format(CultureInfo.InvariantCulture, "IDX10230", jwt))
                        { NotBefore = notBefore, Expires = expires });
                }
                else
                {
                    ValidateLifetime(notBefore: notBefore, expires: expires, securityToken: jwt, validationParameters: validationParameters);
                }
            }

            validatedToken = jwt;
            ClaimsIdentity identity = CreateClaimsIdentity(jwt, jwt.Issuer, validationParameters);
            return new ClaimsPrincipal(identity);
        }
    }
}