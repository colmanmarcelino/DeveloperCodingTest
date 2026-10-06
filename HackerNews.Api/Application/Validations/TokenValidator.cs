using FluentValidation;
using HackerNews.Api.Application.Features.Auth.Commands;

namespace HackerNews.Api.Application.Validations;

public class TokenValidator : AbstractValidator<CreateTokenCommand>
{
    public TokenValidator()
    {
        RuleFor(request => request.Credentials).NotNull();

        When(request => request.Credentials is not null, () =>
        {
            RuleFor(request => request.Credentials.User).NotEmpty().MaximumLength(100);
            RuleFor(request => request.Credentials.Password).NotEmpty().MaximumLength(256);
        });
    }
}
