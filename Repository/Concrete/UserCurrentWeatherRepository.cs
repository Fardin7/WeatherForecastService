using DataAccess;
using Domain.Interface;
using Domain.Models;

namespace WeatherForecastService.Repository
{
    public class UserCurrentWeatherRepository : IUserCurrentWeatherRepository
    {
        private readonly ApiDbContext _apiDbContext;

        public UserCurrentWeatherRepository(ApiDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }
        public async Task Create(UserCurrentWeather userCurrentWeather)
        {
            await _apiDbContext.userCurrentWeather.AddAsync(userCurrentWeather);

            await _apiDbContext.SaveChangesAsync();
        }
    }
}
