using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using TestIdentityServer.ViewModels;
using System.Linq;
using TestIdentityServer.Models;
using IdentityModel;

namespace TestIdentityServer.Controllers
{
    [ApiController]
    [Route("api/weather")]
    [Authorize(AuthenticationSchemes ="Bearer")]
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


        [HttpGet]
        public WeatherForecastViewModel Get()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
            var provinceName = User.Claims.FirstOrDefault(x => x.Type == QvaCarClaims.Province)?.Value;
         
            var rng = new Random();
            var items = Enumerable.Range(1, 5).Select(index => new WeatherForecastItemViewModel
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToList();

            return new WeatherForecastViewModel()
            {
                UserId = userId,
                ProvinceName = provinceName,
                Items = items,
            };
        }
    }
}
