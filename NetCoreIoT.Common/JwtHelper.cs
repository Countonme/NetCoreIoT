using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.Common
{
    public class JwtHelper
    {
        /// <summary>
        /// JwtService
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="issuer">issuer</param>
        /// <param name="audience">audience</param>
        public JwtHelper()
        {
        }
        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="issuer">issuer<param>
        /// <param name="audience">audience</param>
        /// <param name="username">username</param>
        /// <param name="expirationMinutes"></param>
        /// <returns></returns>
        public string GenerateToken(string secretKey, string issuer, string audience, string username, int expirationMinutes = 60)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim("username",username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Token解析
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="issuer">issuer</param>
        /// <param name="audience">audience</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public ClaimsPrincipal DecodeToken(string secretKey, string issuer, string audience, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidIssuer = issuer,
                ValidAudience = audience
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception)
            {
                // Token validation failed
                return null;
            }
        }
    }
}
