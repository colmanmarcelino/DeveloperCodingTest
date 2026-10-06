namespace HackerNews.Api.Application.Options;
public class HackerNewsOptions
{
 public string BestStoriesUrl { get; set; } = "";
 public string ItemUrlTemplate { get; set; } = "";
 public int MaxStories { get; set; } = 500;
 public int CacheSeconds { get; set; } = 60;
}
