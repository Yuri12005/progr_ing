using Serilog;
using System.Diagnostics;

namespace BrainBurst.WebUI
{
    /// <summary>
    /// Middleware ??? ??????????????? ????????? HTTP ??????? ?? ??????????
    /// </summary>
    public class HttpRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpRequestLoggingMiddleware> _logger;

        public HttpRequestLoggingMiddleware(RequestDelegate next, ILogger<HttpRequestLoggingMiddleware> logger)
        {
            _next = next;
          _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
    {
            var stopwatch = Stopwatch.StartNew();
 var request = context.Request;
            
        var requestId = context.TraceIdentifier;
       var remoteIP = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
         var userAgent = request.Headers.UserAgent.ToString();

    try
        {
          _logger.LogInformation(
     "?? HTTP ?????: {Method} {Path}{QueryString} | IP: {RemoteIP} | UserAgent: {UserAgent}",
 request.Method,
    request.Path,
      request.QueryString,
       remoteIP,
userAgent);

       await _next(context);

       stopwatch.Stop();

         var statusCode = context.Response.StatusCode;
   var elapsedMs = stopwatch.ElapsedMilliseconds;

         if (statusCode >= 500)
{
    _logger.LogError(
           "?? HTTP ??????? ???????: {Method} {Path} | Status: {StatusCode} | ???: {ElapsedMs}ms | RequestId: {RequestId}",
            request.Method,
   request.Path,
     statusCode,
   elapsedMs,
requestId);
      }
       else if (statusCode >= 400)
     {
            _logger.LogWarning(
           "?? HTTP ??????? ???????: {Method} {Path} | Status: {StatusCode} | ???: {ElapsedMs}ms | RequestId: {RequestId}",
  request.Method,
       request.Path,
       statusCode,
 elapsedMs,
    requestId);
        }
 else if (elapsedMs > 5000)
     {
        _logger.LogWarning(
  "?? ????????? HTTP ?????: {Method} {Path} | Status: {StatusCode} | ???: {ElapsedMs}ms | RequestId: {RequestId}",
    request.Method,
               request.Path,
          statusCode,
           elapsedMs,
         requestId);
      }
                else
            {
     _logger.LogInformation(
    "?? HTTP ?????: {Method} {Path} | Status: {StatusCode} | ???: {ElapsedMs}ms | RequestId: {RequestId}",
     request.Method,
       request.Path,
      statusCode,
            elapsedMs,
     requestId);
      }
   }
    catch (Exception ex)
            {
 stopwatch.Stop();
         _logger.LogError(ex, "?? ??????????? ??????? ??? ??????? ??????: {Path} | ???: {ElapsedMs}ms | RequestId: {RequestId}",
           request.Path, stopwatch.ElapsedMilliseconds, requestId);
    throw;
            }
        }
    }
}
