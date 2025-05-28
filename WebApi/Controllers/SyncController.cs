using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SyncController(ILogger<SyncController> logger) : Controller
{
    [HttpPost("{count}")]
    public IActionResult Run(int count)
    {
        Stopwatch timer = new();
        timer.Start();
        
        for (int number = 1; number <= count; number++)
        {
            logger.LogInformation($">>> Created {number}");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            
            logger.LogInformation($"<<< Process: {number}");
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        timer.Stop();
        logger.LogInformation($"{timer.ElapsedMilliseconds} ms");
        
        return Ok(timer.ElapsedMilliseconds);
    }
}