namespace HackerNews.Api.Domain.Interfaces;
public interface IHistoryRepository
{
     Task<List<History>> GetBestAsync(CancellationToken cancellationToken);
     Task<Item> GetItemAsync(string itemUrl, CancellationToken cancellationToken);
}
