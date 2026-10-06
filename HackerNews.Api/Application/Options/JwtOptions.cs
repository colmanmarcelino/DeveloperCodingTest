namespace HackerNews.Api.Application.Options;
public class JwtOptions
{
 public string Issuer { get; set; } = "HackerNews";
 public string Audience { get; set; } = "HackerNewsClients";
 public string SigningKey { get; set; } = "";
 public int ExpirationMinutes { get; set; } = 30;
}
