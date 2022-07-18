using Microsoft.AspNetCore.Identity;

namespace WeatherForecastService.Model
{
    public class User:IdentityUser
    {
        public ICollection<UserCurrentWeather> userCurrentWeather { get; set; }
    }
}
