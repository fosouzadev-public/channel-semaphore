using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ChannelController(ILogger<ChannelController> logger) : ControllerBase
{
    private Channel<long> _channel;
    
    [HttpPost("{count}/size/{size}")]
    public IActionResult RunAsync(int count, int size)
    {
        Stopwatch timer = new();
        timer.Start();

        _channel = Channel.CreateBounded<long>(size);
        
        Task.WaitAll(
         GenerateNumbers(count),
         ConsumeNumbers());

        timer.Stop();
        logger.LogInformation($"{timer.ElapsedMilliseconds} ms");
        
        return Ok(timer.ElapsedMilliseconds);
    }
    
    private async Task GenerateNumbers(int count)
    {
        try
        {
            for (int i = 1; i <= count; i++)
            {
                await _channel.Writer.WriteAsync(i);
                logger.LogInformation($">>> Published {i}");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        finally
        {
            _channel.Writer.Complete();
        }
    }
    
    private async Task ConsumeNumbers()
    {
        await foreach (long number in _channel.Reader.ReadAllAsync())
        {
            logger.LogInformation($"<<< Number: {number}");
            logger.LogInformation($"### Count: {_channel.Reader.Count}");
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}