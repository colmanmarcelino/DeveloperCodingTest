using HackerNews.Api.Application.DTOs;
using HackerNews.Api.Application.Interfaces;
using MediatR;

namespace HackerNews.Api.Application.Features.Stories.Queries;

public class GetBestStoriesHandler : IRequestHandler<GetBestStoriesQuery, List<ItemDTO>>
{
    private readonly IStoryService _storyService;

    public GetBestStoriesHandler(IStoryService storyService)
    {
        _storyService = storyService;
    }

    public Task<List<ItemDTO>> Handle(GetBestStoriesQuery request, CancellationToken cancellationToken)
        => _storyService.GetAsync(request.N, cancellationToken);
}