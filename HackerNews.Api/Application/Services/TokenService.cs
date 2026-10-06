using System.Security.Cryptography;
using System.Text;
using HackerNews.Api.Application.DTOs;
using HackerNews.Api.Application.Exceptions;
using HackerNews.Api.Application.Interfaces;
using HackerNews.Api.Application.Options;
using Microsoft.Extensions.Options;
namespace HackerNews.Api.Application.Services;
// Authentication decision belongs to the application layer; signing belongs to infrastructure.
public class TokenService(IOptions<AuthorizationOptions> options, IJwtTokenIssuer tokenIssuer) : ITokenService
{
 public TokenResponseDTO Create(TokenRequestDTO request)
 {
  var settings = options.Value;
  var validUser = CryptographicOperations.FixedTimeEquals(
   SHA256.HashData(Encoding.UTF8.GetBytes(request.User)),
   SHA256.HashData(Encoding.UTF8.GetBytes(settings.User)));
  var validPassword = CryptographicOperations.FixedTimeEquals(
   SHA256.HashData(Encoding.UTF8.GetBytes(request.Password)),
   SHA256.HashData(Encoding.UTF8.GetBytes(settings.Password)));
  if(!(validUser & validPassword)) throw new InvalidCredentialsException();
  return tokenIssuer.Issue(settings.User);
 }
}
