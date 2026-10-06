using FluentValidation;
using HackerNews.Api.Application.Features.Stories.Queries;
using HackerNews.Api.Application.Options;
using Microsoft.Extensions.Options;

namespace HackerNews.Api.Application.Validations;

public class BestStoriesValidator : AbstractValidator<GetBestStoriesQuery>
{
    public BestStoriesValidator(IOptions<HackerNewsOptions> options)
    {
        RuleFor(request => request.N).InclusiveBetween(1, options.Value.MaxStories);
    }
}
