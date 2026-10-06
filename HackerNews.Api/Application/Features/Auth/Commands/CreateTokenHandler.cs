using HackerNews.Api.Application.DTOs;
using HackerNews.Api.Application.Interfaces;
using MediatR;

namespace HackerNews.Api.Application.Features.Auth.Commands;

public class CreateTokenHandler : IRequestHandler<CreateTokenCommand, TokenResponseDTO>
{
    private readonly ITokenService _tokenService;

    public CreateTokenHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public Task<TokenResponseDTO> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
        => Task.FromResult(_tokenService.Create(request.Credentials));
}
