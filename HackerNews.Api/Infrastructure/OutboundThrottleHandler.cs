namespace HackerNews.Api.Infrastructure;
// Registered inside resilience: every attempt, including a retry, passes this gate.
public sealed class OutboundThrottleHandler : DelegatingHandler
{
 private static readonly SemaphoreSlim requestGate = new(1,1);
 private static long nextAttemptAt;
 protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
 {
  await requestGate.WaitAsync(cancellationToken);
  try
  {
   var delay = nextAttemptAt - Environment.TickCount64;
   if(delay>0) await Task.Delay(TimeSpan.FromMilliseconds(delay),cancellationToken);
   nextAttemptAt = Environment.TickCount64+100; // at most 10 starts/sec/process
   return await base.SendAsync(request,cancellationToken);
  }
  finally { requestGate.Release(); }
 }
}
