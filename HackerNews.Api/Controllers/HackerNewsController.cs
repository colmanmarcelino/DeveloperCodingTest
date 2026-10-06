using HackerNews.Api.Application.DTOs;
using HackerNews.Api.Application.Features.Stories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HackerNews.Api.Controllers;
[ApiController]
[Route("api/hackernews")]
[Authorize]
public class HackerNewsController(ISender sender) : ControllerBase
{
 [HttpGet("best-stories")]
 [ProducesResponseType(typeof(List<ItemDTO>),StatusCodes.Status200OK)]
 [ProducesResponseType(StatusCodes.Status400BadRequest)]
 [ProducesResponseType(StatusCodes.Status401Unauthorized)]
 [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
 public async Task<ActionResult<List<ItemDTO>>> Get([FromQuery] BestStoriesRequestDTO request,CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetBestStoriesQuery(request.N),cancellationToken));
}
