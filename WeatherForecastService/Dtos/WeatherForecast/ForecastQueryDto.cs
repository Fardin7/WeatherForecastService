using System.ComponentModel.DataAnnotations;

namespace WeatherForecastService.Dtos.WeatherForecast
{
    public class ForecastQueryDto
    {
        [Required]
        public string City { get; set; }
    }
}
