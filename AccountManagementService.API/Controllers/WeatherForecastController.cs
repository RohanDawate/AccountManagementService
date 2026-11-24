using AccountManagementService.Business;
using AccountManagementService.CrossCutting;
using AccountManagementService.CrossCutting.Logging;
using Microsoft.AspNetCore.Mvc;

namespace AccountManagementService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class WeatherForecastController : AppControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            SetRecordLog("AccountNo", "12121");

            Acc acc = new Acc();
            string s = acc.GetData("Raj", 21);

            Customer c = acc.GetCustomer(1);

            string a = FormData();
            string aaa = GetData("s1");

            _logger.DevInfo("WeatherForecastController Get() called");
            _logger.DevDebug("Payload received: {@Request}", "a");
            _logger.BusinessInfo("Transfer initiated for amount {Amount}", "ff");
            _logger.AuditInfo("User initiated account transfer");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        public string FormData()
        {
            string s = "Learn";
            return s;
        }

        public string GetData(string s1)
        {
            Acc acc = new Acc();
            string s = acc.GetData("Raj", 21);

            return s1 + " Hello";
        }
    }
}
