using System.ComponentModel.DataAnnotations;

namespace WeatherForecastService.Dtos
{
    public class ForcastDto
    {
        [Required]
        public string City { get; set; }
    }
}
