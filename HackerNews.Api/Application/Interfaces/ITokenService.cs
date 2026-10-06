using HackerNews.Api.Application.DTOs;

namespace HackerNews.Api.Application.Interfaces;

public interface ITokenService
{
    TokenResponseDTO Create(TokenRequestDTO request);
}
