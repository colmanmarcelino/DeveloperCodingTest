namespace HackerNews.Api.Application.Exceptions;

public class UpstreamException(string message, Exception? inner = null)
    : Exception(
        string.IsNullOrWhiteSpace(message)
            ? "Hacker News is temporarily unavailable. Please try again later."
            : message.Trim(),
        inner)
{
}
