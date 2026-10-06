using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace HackerNews.Api.Filters;
public class JwtOperationFilter : IOperationFilter
{
 public void Apply(OpenApiOperation operation,OperationFilterContext context)
 {
  if(context.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute),true)) return;
  operation.Security=[new OpenApiSecurityRequirement { [new OpenApiSecurityScheme { Reference=new OpenApiReference { Type=ReferenceType.SecurityScheme,Id="Bearer" } }]=Array.Empty<string>() }];
 }
}
