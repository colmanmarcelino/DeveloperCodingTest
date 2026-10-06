using HackerNews.Api.Application.DTOs;
using HackerNews.Api.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace HackerNews.Api.Controllers;
[ApiController]
[Route("api/authorization")]
public class AuthorizationController(ISender sender) : ControllerBase
{
 [AllowAnonymous]
 [HttpGet("token")]
 [EnableRateLimiting("token")]
 [ResponseCache(NoStore=true,Location=ResponseCacheLocation.None)]
 [ProducesResponseType(typeof(TokenResponseDTO),StatusCodes.Status200OK)]
 [ProducesResponseType(StatusCodes.Status400BadRequest)]
 [ProducesResponseType(StatusCodes.Status401Unauthorized)]
 public async Task<ActionResult<TokenResponseDTO>> GetToken([FromQuery] TokenRequestDTO request,CancellationToken cancellationToken)
 {
  Response.Headers.CacheControl="no-store";
  Response.Headers.Pragma="no-cache";
  return Ok(await sender.Send(new CreateTokenCommand(request),cancellationToken));
 }
}
