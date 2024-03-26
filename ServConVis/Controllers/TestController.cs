using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ServConVis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {

        private HttpClient _client;
        private readonly ILogger<WeatherForecastController> _logger;

        public TestController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
        }

        [HttpGet(Name = "Test")]
        public string? Get()
        {
            var x = this._client.GetAsync("https://api.coindesk.com/v1/bpi/currentprice.json").Result;
            return x?.Content?.ReadAsStringAsync().Result;
        }
    }
}