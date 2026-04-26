using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BrainBurst.WebUI.Models;
using Serilog.Context;

namespace BrainBurst.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
     _logger = logger;
 }

    public IActionResult Index()
    {
        var stopwatch = Stopwatch.StartNew();
   var operationId = Guid.NewGuid().ToString();
      
        using (LogContext.PushProperty("OperationId", operationId))
        using (LogContext.PushProperty("OperationName", "Index"))
using (LogContext.PushProperty("UserId", HttpContext.User.Identity?.Name ?? "Anonymous"))
        {
       try
            {
            _logger.LogInformation("START: User navigating to home page");
     _logger.LogDebug("Debug: Index operation started");
       
   // Simulate some work
   Thread.Sleep(50);
      
      stopwatch.Stop();
var elapsedMs = stopwatch.ElapsedMilliseconds;
       
      if (elapsedMs > 1000)
     {
        _logger.LogWarning("SLOW: Index operation took {ElapsedMs}ms", elapsedMs);
 }
           else
      {
         _logger.LogInformation("SUCCESS: Operation completed in {ElapsedMs}ms", elapsedMs);
      }
      
     return View();
            }
catch (Exception ex)
            {
                stopwatch.Stop();
       _logger.LogError(ex, "ERROR: Critical error on Index page. Time: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
     throw;
        }
            finally
     {
     _logger.LogInformation("FINISH: Index operation completed");
        }
        }
    }

    public IActionResult Privacy()
  {
        var stopwatch = Stopwatch.StartNew();
   var operationId = Guid.NewGuid().ToString();
      
        using (LogContext.PushProperty("OperationId", operationId))
  using (LogContext.PushProperty("OperationName", "Privacy"))
     {
      try
      {
         _logger.LogInformation("START: User navigating to privacy page");
     
                Thread.Sleep(30);
 
                stopwatch.Stop();
    _logger.LogInformation("SUCCESS: Privacy page loaded in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
  
          return View();
     }
         catch (Exception ex)
       {
    stopwatch.Stop();
          _logger.LogError(ex, "ERROR: Error on Privacy page. Time: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
          throw;
  }
            finally
       {
       _logger.LogInformation("FINISH: Privacy operation completed");
            }
     }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
     
        _logger.LogError("ERROR: Error page activated. RequestId: {RequestId}", requestId);

        return View(new ErrorViewModel { RequestId = requestId });
    }
}
