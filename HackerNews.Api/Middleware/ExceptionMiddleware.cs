using HackerNews.Api.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
namespace HackerNews.Api.Middleware;
public class ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
{
 public async Task InvokeAsync(HttpContext context)
 {
  try { await next(context); }
  catch(OperationCanceledException) when(context.RequestAborted.IsCancellationRequested) { }
  catch(Exception exception)
  {
   if(context.Response.HasStarted) throw;
   var status = exception switch { ValidationException=>400,InvalidCredentialsException=>401,UpstreamException=>503,_=>500 };
   logger.LogError(exception,"Request failed. TraceId: {TraceId}",context.TraceIdentifier);
   context.Response.StatusCode=status;
   if(status==503) context.Response.Headers.RetryAfter="5";
   var problem = new ProblemDetails { Status=status,Title=status switch {400=>"Invalid request",401=>"Invalid credentials",503=>"Upstream unavailable",_=>"Unexpected error"} };
   problem.Extensions["traceId"]=context.TraceIdentifier;
   if(exception is ValidationException validation) problem.Extensions["errors"]=validation.Errors.Select(x=>new { field=x.PropertyName,message=x.ErrorMessage });
   await context.Response.WriteAsJsonAsync(problem,options:null,contentType:"application/problem+json",cancellationToken:context.RequestAborted);
  }
 }
}
