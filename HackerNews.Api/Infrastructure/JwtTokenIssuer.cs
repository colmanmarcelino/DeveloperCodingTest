using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HackerNews.Api.Application.DTOs;
using HackerNews.Api.Application.Interfaces;
using HackerNews.Api.Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
namespace HackerNews.Api.Infrastructure;
public class JwtTokenIssuer(IOptions<JwtOptions> options) : IJwtTokenIssuer
{
 public TokenResponseDTO Issue(string user)
 {
      var settings = options.Value;
      var now = DateTime.UtcNow;
      var token = new JwtSecurityToken(settings.Issuer,settings.Audience,
        [new Claim(JwtRegisteredClaimNames.Sub,user),new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())],
         now,now.AddMinutes(settings.ExpirationMinutes),new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)),SecurityAlgorithms.HmacSha256));
      return new TokenResponseDTO(new JwtSecurityTokenHandler().WriteToken(token),"Bearer",settings.ExpirationMinutes*60);
     }
}