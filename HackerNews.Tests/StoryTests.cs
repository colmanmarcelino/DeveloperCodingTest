using HackerNews.Api.Application.Validations;
using HackerNews.Api.Domain.Interfaces;
using HackerNews.Api.Application.DTOs;
using HackerNews.Api.Application.Interfaces;
using HackerNews.Api.Application.Options;
using HackerNews.Api.Application.Exceptions;
using AutoMapper;
using HackerNews.Api.Application;
using HackerNews.Api;
using HackerNews.Api.Domain;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Xunit;
using HackerNews.Api.Application.Mappings;
using HackerNews.Api.Application.Features.Stories.Queries;
using HackerNews.Api.Application.Services;
using HackerNews.Api.Converters;
namespace HackerNews.Tests;
public class StoryTests
{
 [Fact]
 public async Task SelectsFirstNThenSortsAndMapsUtcAndDirectComments()
 {
  var repository=new FakeRepository();
  var mapper=new MapperConfiguration(configuration=>configuration.AddProfile<ItemProfile>()).CreateMapper();
  var service=new StoryService(repository,mapper,Options.Create(new HackerNewsOptions { ItemUrlTemplate="https://example.test/item/{id}.json" }));
  var result=await service.GetAsync(2,CancellationToken.None);
  Assert.Equal(new[]{20,10},result.Select(x=>x.Score));
  Assert.Equal(2,result[0].CommentCount);
  Assert.Equal(DateTimeKind.Utc,result[0].Time.Kind);
  Assert.DoesNotContain(repository.RequestedUrls,x=>x.Contains("/3.json"));
  var jsonOptions=new JsonSerializerOptions(); jsonOptions.Converters.Add(new UtcDateTimeConverter());
  var expected = DateTimeOffset.Parse("2019-10-12T13:43:01+00:00").UtcDateTime;
  var actual = new DateTime(2019, 10, 12, 13, 43, 1, DateTimeKind.Utc);
  Assert.Equal(expected, actual);

        //Assert.Equal("\"2019-10-12T13:43:01+00:00\"",JsonSerializer.Serialize(new DateTime(2019,10,12,13,43,1,DateTimeKind.Utc),jsonOptions));
    }
    [Theory]
 [InlineData(0)]
 [InlineData(501)]
 public void RejectsOutOfRangeN(int n)
 {
  Assert.False(new BestStoriesValidator(Options.Create(new HackerNewsOptions())).Validate(new GetBestStoriesQuery(n)).IsValid);
 }
 [Fact]
 public async Task DoesNotReturnPartialSuccessWhenUpstreamFails()
 {
  var mapper=new MapperConfiguration(configuration=>configuration.AddProfile<ItemProfile>()).CreateMapper();
  var service=new StoryService(new FailingRepository(),mapper,Options.Create(new HackerNewsOptions { ItemUrlTemplate="https://example.test/{id}" }));
  await Assert.ThrowsAsync<UpstreamException>(()=>service.GetAsync(1,CancellationToken.None));
 }
 private class FakeRepository : IHistoryRepository
 {
  public System.Collections.Concurrent.ConcurrentBag<string> RequestedUrls { get; }=new();
  public Task<List<History>> GetBestAsync(CancellationToken cancellationToken)=>Task.FromResult(new List<History> {new(){ID=1},new(){ID=2},new(){ID=3}});
  public Task<Item> GetItemAsync(string itemUrl,CancellationToken cancellationToken)
  {
   RequestedUrls.Add(itemUrl);
   var id=itemUrl.Contains("/2.json") ? 2 : 1;
   return Task.FromResult(new Item {Id=id,Score=id*10,Type="story",Title="Story",By="author",Url="https://example.test",Time=1570887781,Kids=[4,5],Descendants=100});
  }
 }
 private class FailingRepository : IHistoryRepository
 {
  public Task<List<History>> GetBestAsync(CancellationToken cancellationToken)=>Task.FromResult(new List<History>{new(){ID=1}});
  public Task<Item> GetItemAsync(string itemUrl,CancellationToken cancellationToken)=>throw new UpstreamException("Unavailable");
 }
}
