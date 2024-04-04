using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing;
using System;

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
        [HttpPost("read-multiple-enable-buffering")]
        public async Task<IActionResult> ReadMultipleEnableBuffering()
        {
            Request.EnableBuffering();
            var requestBody = await Request.Body.ReadAsStringAsync(true);
            Request.Body.Position = 0;
            var requestBodySecond = await Request.Body.ReadAsStringAsync();
            return Ok($"First: {requestBody}, Second:{requestBodySecond}");
        }
        [HttpPost("read-from-body")]
        public IActionResult ReadFromBody([FromBody] PersonItemDto model)
        {
            var message = $"Person Data => Name: {model.Name}, Age: {model.Age}";
            return Ok(message);
        }
        [HttpPost("read-from-body-multi-param")]
        public IActionResult ReadFromBodyMultiParam([FromBody] PersonItemDto model, [FromBody] decimal salary)
        {
            var message = $"Person Data => Name: {model.Name}, Age: {model.Age}, Salary: {salary}";
            return Ok(message);
        }
        //In simple terms, this exception notifies us that the FromBody attribute is permitted only once in action method parameters.
        //    Hence, it is advisable to gather all parameters within a single request model parameter.

        /********Using a Custom Attribute to Read the Request Body******/
        [ReadRequestBody]
        public IActionResult ReadFromAttribute()
        {
            var requestBody = Request.Headers["ReadRequestBodyAttribute"];
            var message = $"Request Body From Attribute : {requestBody}";
            return Ok(message);
        }
        /********Using a Custom Attribute to Read the Request Body******/
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
    public class PersonItemDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
