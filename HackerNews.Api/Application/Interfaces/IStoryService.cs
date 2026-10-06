using HackerNews.Api.Application.DTOs;

namespace HackerNews.Api.Application.Interfaces;

public interface IStoryService
{
    Task<List<ItemDTO>> GetAsync(int n, CancellationToken cancellationToken);
}
