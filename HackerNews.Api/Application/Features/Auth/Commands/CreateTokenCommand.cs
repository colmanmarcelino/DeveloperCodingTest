using HackerNews.Api.Application.DTOs;
using MediatR;

namespace HackerNews.Api.Application.Features.Auth.Commands;

public record CreateTokenCommand(TokenRequestDTO Credentials) : IRequest<TokenResponseDTO>;
