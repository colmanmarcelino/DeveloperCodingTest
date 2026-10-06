using HackerNews.Api.Application.DTOs;
using HackerNews.Api.Application.Interfaces;
using HackerNews.Api.Application.Options;
using HackerNews.Api.Application.Exceptions;
using HackerNews.Api.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;
namespace HackerNews.Api.Application.Services;
public class StoryService(IHistoryRepository repository,IMapper mapper,IOptions<HackerNewsOptions> options) : IStoryService
{
 public async Task<List<ItemDTO>> GetAsync(int n,CancellationToken cancellationToken)
 {
  var histories = (await repository.GetBestAsync(cancellationToken)).Take(n).ToList();
  if(histories.Count<n) throw new FluentValidation.ValidationException("Requested n exceeds the number of available IDs.");
  var items = new Domain.Item[histories.Count];
  await Parallel.ForEachAsync(Enumerable.Range(0,histories.Count),new ParallelOptions { MaxDegreeOfParallelism=8,CancellationToken=cancellationToken },async(index,token)=>
  {
   var itemUrl = options.Value.ItemUrlTemplate.Replace("{id}",histories[index].ID.ToString(System.Globalization.CultureInfo.InvariantCulture));
   items[index] = await repository.GetItemAsync(itemUrl,token);
  });
  if(items.Any(x=>x.Deleted || x.Dead || x.Type!="story")) throw new UpstreamException("The selected snapshot contains unavailable stories.");
  return mapper.Map<List<ItemDTO>>(items.OrderByDescending(x=>x.Score).ThenBy(x=>x.Id).ToList());
 }
}
