using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SemaphoreSlimController(ILogger<SemaphoreSlimController> logger) : Controller
{
    private SemaphoreSlim _semaphoreSlim;
    
    [HttpPost("{count}/concurrently/{maxConcurrently}")]
    public async Task<IActionResult> RunAsync(int count, int maxConcurrently)
    {
        Stopwatch timer = new();
        timer.Start();

        _semaphoreSlim = new(maxConcurrently);
        
        List<Task> tasks = new();
        for (int number = 1; number <= count; number++)
        {
            tasks.Add(ProcessAsync(number));
        }

        await Task.WhenAll(tasks);
        logger.LogInformation($"... Waiting : {DateTime.Now:hh:mm:ss:fff}");
        
        timer.Stop();
        logger.LogInformation($"{timer.ElapsedMilliseconds} ms");
        
        return Ok(timer.ElapsedMilliseconds);
    }

    private async Task ProcessAsync(int number)
    {
        await _semaphoreSlim.WaitAsync();
        
        logger.LogInformation($">>> Created {number} : {DateTime.Now:hh:mm:ss:fff}");
        await Task.Delay(TimeSpan.FromSeconds(1));
        
        logger.LogInformation($"<<< Process: {number} : {DateTime.Now:hh:mm:ss:fff}");
        await Task.Delay(TimeSpan.FromSeconds(2));
        
        _semaphoreSlim.Release();
    }
}