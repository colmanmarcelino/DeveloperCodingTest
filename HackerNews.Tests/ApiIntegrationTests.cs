using Autofac;
using HackerNews.Api.Domain;
using HackerNews.Api.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace HackerNews.Tests;
public class ApiIntegrationTests : IClassFixture<DemoApiFactory>
{
    private readonly HttpClient client;
    public ApiIntegrationTests(DemoApiFactory factory) => client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

 [Theory]
 [InlineData("demo","wrong")]
 [InlineData("wrong","demo")]
 public async Task InvalidCredentialsReturn401(string user,string password)
 {
  var response=await client.GetAsync($"/api/authorization/token?user={user}&password={password}");
  Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
 }
 [Fact]
 public async Task MissingPasswordReturns400()
 {
  var response=await client.GetAsync("/api/authorization/token?user=demo");
  Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
 }
 [Fact]
 public async Task StoriesWithoutTokenReturn401()
 {
  var response=await client.GetAsync("/api/hackernews/best-stories?n=2");
  Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
 }
 [Fact]
 public async Task InvalidBearerTokenReturns401()
 {
  using var request=new HttpRequestMessage(HttpMethod.Get,"/api/hackernews/best-stories?n=2");
  request.Headers.Authorization=new AuthenticationHeaderValue("Bearer","invalid-token");
  Assert.Equal(HttpStatusCode.Unauthorized,(await client.SendAsync(request)).StatusCode);
 }
 [Fact]
 public async Task OldTokenRouteIsRemoved()
 {
  var response=await client.PostAsJsonAsync("/api/hackernews/token",new {username="demo",password="demo"});
  Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);
 }
 [Fact]
 public async Task SwaggerDefinesAnonymousTokenAndProtectedStories()
 {
  var json=await client.GetStringAsync("/swagger/v1/swagger.json");
  using var document=JsonDocument.Parse(json);
  var paths=document.RootElement.GetProperty("paths");
  var token=paths.GetProperty("/api/authorization/token").GetProperty("get");
  Assert.False(token.TryGetProperty("security",out var security) && security.GetArrayLength()>0);
  var stories=paths.GetProperty("/api/hackernews/best-stories").GetProperty("get");
  Assert.True(stories.GetProperty("security").GetArrayLength()>0);
 }
}
public class DemoApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureTestContainer<ContainerBuilder>(container => container.RegisterType<IntegrationRepository>().As<IHistoryRepository>().SingleInstance());
    }
    private class IntegrationRepository : IHistoryRepository
    {
        public IntegrationRepository() { }
        public Task<List<History>> GetBestAsync(CancellationToken cancellationToken) => Task.FromResult(new List<History> { new() { ID = 1 }, new() { ID = 2 } });
        public Task<Item> GetItemAsync(string itemUrl, CancellationToken cancellationToken)
        {
            var id = itemUrl.EndsWith("/2.json", StringComparison.Ordinal) ? 2 : 1;
            return Task.FromResult(new Item { Id = id, Score = id * 10, Title = "Test story", Type = "story", By = "author", Kids = [3], Time = 1570887781 });
        }
    }
}
