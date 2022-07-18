using DataAccess;
using Domain.Interface;
using Domain.Models;

namespace WeatherForecastService.Repository
{
    public class CurrentWeatherRepository : ICurrentWeatherRepository
    {
        private readonly ApiDbContext _apiDbContext;

        public CurrentWeatherRepository(ApiDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }
        public async Task<CurrentWeather> CreateAsync(CurrentWeather CurrentWeather)
        {
            await _apiDbContext.currentWeather.AddAsync(CurrentWeather);

            await _apiDbContext.SaveChangesAsync();

            return CurrentWeather;
        }
    }
}
