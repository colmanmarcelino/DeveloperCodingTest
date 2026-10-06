using HackerNews.Api.Application.Behaviors;
using HackerNews.Api.Application.Validations;
using HackerNews.Api.Application.Interfaces;
using HackerNews.Api.Application.Options;
using HackerNews.Api.Domain.Interfaces;
using System.Text;
using System.Threading.RateLimiting;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using HackerNews.Api.Middleware;
using HackerNews.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using HackerNews.Api.Application.Mappings;
using HackerNews.Api.Application.Features.Stories.Queries;
using HackerNews.Api.Application.Services;
using HackerNews.Api.Filters;
using HackerNews.Api.Converters;
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container=>
{
 container.RegisterType<HistoryRepository>().As<IHistoryRepository>().SingleInstance();
 container.RegisterType<StoryService>().As<IStoryService>().InstancePerLifetimeScope();
 container.RegisterType<TokenService>().As<ITokenService>().InstancePerLifetimeScope();
 container.RegisterType<JwtTokenIssuer>().As<IJwtTokenIssuer>().InstancePerLifetimeScope();
});
builder.Host.UseSerilog((context,services,configuration)=>configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services).Enrich.FromLogContext().WriteTo.Console());
builder.Services.AddOptions<HackerNewsOptions>().BindConfiguration("HackerNews")
 .Validate(x=>Uri.TryCreate(x.BestStoriesUrl,UriKind.Absolute,out var url) && url.Scheme=="https" && x.ItemUrlTemplate.Contains("{id}") && Uri.TryCreate(x.ItemUrlTemplate.Replace("{id}","1"),UriKind.Absolute,out var itemUrl) && itemUrl.Scheme=="https" && x.MaxStories is >0 and <=500 && x.CacheSeconds>0,"Invalid Hacker News configuration").ValidateOnStart();
builder.Services.AddOptions<JwtOptions>().BindConfiguration("Jwt")
 .Validate(x=>Encoding.UTF8.GetByteCount(x.SigningKey)>=32 && !string.IsNullOrWhiteSpace(x.Issuer) && !string.IsNullOrWhiteSpace(x.Audience) && x.ExpirationMinutes is >0 and <=60,"Configure JWT secrets before startup").ValidateOnStart();
builder.Services.AddOptions<HackerNews.Api.Application.Options.AuthorizationOptions>().BindConfiguration("Authorization")
 .Validate(x=>!string.IsNullOrWhiteSpace(x.User) && !string.IsNullOrWhiteSpace(x.Password),"Configure the demo user and password").ValidateOnStart();
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options=>
{
 options.TokenValidationParameters=new TokenValidationParameters { ValidateIssuer=true,ValidateAudience=true,ValidateLifetime=true,ValidateIssuerSigningKey=true,ValidIssuer=jwt.Issuer,ValidAudience=jwt.Audience,IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),ClockSkew=TimeSpan.FromSeconds(30),ValidAlgorithms=[SecurityAlgorithms.HmacSha256] };
});
builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(options=>options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter()));
builder.Services.AddAutoMapper(typeof(ItemProfile));
builder.Services.AddMediatR(configuration=> { configuration.RegisterServicesFromAssemblyContaining<GetBestStoriesQuery>(); configuration.AddOpenBehavior(typeof(ValidationBehavior<,>)); });
builder.Services.AddValidatorsFromAssemblyContaining<BestStoriesValidator>();
builder.Services.AddTransient<OutboundThrottleHandler>();
var httpBuilder = builder.Services.AddHttpClient("hackerNews",client=>client.Timeout=Timeout.InfiniteTimeSpan);
httpBuilder.AddStandardResilienceHandler(options=>
{
 options.Retry.MaxRetryAttempts=3;
 options.Retry.Delay=TimeSpan.FromMilliseconds(500);
 options.Retry.UseJitter=true;
 options.AttemptTimeout.Timeout=TimeSpan.FromSeconds(8);
 options.TotalRequestTimeout.Timeout=TimeSpan.FromSeconds(40);
 options.CircuitBreaker.SamplingDuration=TimeSpan.FromSeconds(30);
});
httpBuilder.AddHttpMessageHandler<OutboundThrottleHandler>();
builder.Services.AddRateLimiter(options=>
{
 options.RejectionStatusCode=429;
 options.GlobalLimiter=PartitionedRateLimiter.Create<HttpContext,string>(_=>RateLimitPartition.GetConcurrencyLimiter("global",_=>new ConcurrencyLimiterOptions { PermitLimit=100,QueueLimit=0 }));
 options.AddPolicy("token",context=>RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown",_=>new FixedWindowRateLimiterOptions { PermitLimit=5,Window=TimeSpan.FromMinutes(1),QueueLimit=0 }));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options=>
{
 options.SwaggerDoc("v1",new OpenApiInfo { Title="Hacker News API",Version="v1" });
 options.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme { Type=SecuritySchemeType.Http,Scheme="bearer",BearerFormat="JWT",Description="Paste the accessToken returned by GET /api/authorization/token" });
 options.OperationFilter<JwtOperationFilter>();
});
var app = builder.Build();
app.UseSerilogRequestLogging(options=>
{
 options.IncludeQueryInRequestPath=false;
 options.EnrichDiagnosticContext=(diagnostics,context)=>diagnostics.Set("TraceId",context.TraceIdentifier);
});
app.UseMiddleware<ExceptionMiddleware>();
if(app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
if(!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
public partial class Program { }
