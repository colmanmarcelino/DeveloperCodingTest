namespace HackerNews.Api.Application.DTOs;

public record TokenResponseDTO(string AccessToken, string TokenType, int ExpiresIn);
