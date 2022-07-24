using Domain.Models;

namespace Domain.Interface
{
    public interface ICurrentWeatherRepository
    {
        Task<CurrentWeather> CreateAsync(CurrentWeather CurrentWeather); 
    }
}
