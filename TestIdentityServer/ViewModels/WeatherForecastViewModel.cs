using System.Collections.Generic;
using TestIdentityServer.Models;

namespace TestIdentityServer.ViewModels
{
    public class WeatherForecastViewModel
    {
        public string UserId { get; set; }
        public string ProvinceName { get; set; }

        public IEnumerable<WeatherForecastItemViewModel> Items { get; set; } = new List<WeatherForecastItemViewModel>();
    }
}
