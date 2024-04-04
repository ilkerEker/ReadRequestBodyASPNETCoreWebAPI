using Microsoft.AspNetCore.Mvc;

namespace ReadRequestBodyASPNETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
        [HttpPost("read-as-string")]
        public async Task<IActionResult> ReadAsString()
        {
            var requestBody = await Request.Body.ReadAsStringAsync();

            return Ok($"Request Body As String: {requestBody}");
        }
        [HttpPost("read-as-string-multiple")]
        public async Task<IActionResult> ReadAsStringMultiple()
        {
            var requestBody = await Request.Body.ReadAsStringAsync();
            var requestBodySecond = await Request.Body.ReadAsStringAsync();
            return Ok($"First: {requestBody}, Second:{requestBodySecond}");
        }
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
    public static class RequestExtensions
    {
        public static async Task<string> ReadAsStringAsync(this Stream requestBody, bool leaveOpen = false)
        {
            using StreamReader reader = new(requestBody, leaveOpen: leaveOpen);
            var bodyAsString = await reader.ReadToEndAsync();
            return bodyAsString;
        }
    }
}
