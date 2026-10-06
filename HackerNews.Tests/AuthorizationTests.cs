using HackerNews.Api.Application;
using HackerNews.Api.Application.DTOs;
using HackerNews.Api.Application.Exceptions;
using HackerNews.Api.Application.Interfaces;
using HackerNews.Api.Application.Options;
using HackerNews.Api.Application.Services;
using Microsoft.Extensions.Options;
using Xunit;
namespace HackerNews.Tests;
public class AuthorizationTests
{
 [Fact]
 public void CorrectCredentialsDelegateToIssuer()
 {
  var issuer=new FakeIssuer();
  var service=new TokenService(Options.Create(new AuthorizationOptions { User="demo",Password="demo" }),issuer);
  var result=service.Create(new TokenRequestDTO("demo","demo"));
  Assert.Equal("demo",issuer.IssuedFor);
  Assert.Equal("test-token",result.AccessToken);
 }
 [Theory]
 [InlineData("demo","wrong")]
 [InlineData("wrong","demo")]
 [InlineData("DEMO","demo")]
 public void InvalidCredentialsNeverIssueAToken(string user,string password)
 {
  var issuer=new FakeIssuer();
  var service=new TokenService(Options.Create(new AuthorizationOptions {User="demo",Password="demo"}),issuer);
  Assert.Throws<InvalidCredentialsException>(()=>service.Create(new TokenRequestDTO(user,password)));
  Assert.Null(issuer.IssuedFor);
 }
 private class FakeIssuer : IJwtTokenIssuer
 {
  public string? IssuedFor { get; private set; }
  public TokenResponseDTO Issue(string user) { IssuedFor=user; return new TokenResponseDTO("test-token","Bearer",1800); }
 }
}
