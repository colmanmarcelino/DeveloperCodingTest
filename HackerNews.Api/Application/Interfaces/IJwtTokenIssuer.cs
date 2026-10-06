using HackerNews.Api.Application.DTOs;

namespace HackerNews.Api.Application.Interfaces;

public interface IJwtTokenIssuer
{
    TokenResponseDTO Issue(string user);
}
