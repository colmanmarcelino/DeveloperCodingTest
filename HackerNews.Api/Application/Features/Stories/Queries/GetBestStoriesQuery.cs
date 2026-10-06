using HackerNews.Api.Application.DTOs;
using MediatR;

namespace HackerNews.Api.Application.Features.Stories.Queries;

public record GetBestStoriesQuery(int N) : IRequest<List<ItemDTO>>;