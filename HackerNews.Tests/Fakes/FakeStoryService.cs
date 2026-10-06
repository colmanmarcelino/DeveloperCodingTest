using HackerNews.Api.Application.DTOs;
using HackerNews.Api.Application.Interfaces;

namespace HackerNews.Tests.Fakes
{
    public class FakeStoryService : IStoryService
    {
        public Task<List<ItemDTO>> GetAsync(int n, CancellationToken ct)
            => Task.FromResult(new List<ItemDTO>
            {
            new ItemDTO { Score = 20 },
            new ItemDTO { Score = 10 }
            });
    }
}
