using DataAccess;
using Domain.Interface;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WeatherForecastService.Repository
{
    public class UserCurrentWeatherRepository : IUserCurrentWeatherRepository
    {
        private readonly ApiDbContext _apiDbContext;
        private readonly IConfiguration _configuration;

        public UserCurrentWeatherRepository(ApiDbContext apiDbContext,IConfiguration configuration)
        {
            _apiDbContext = apiDbContext;
            _configuration = configuration;
        }
        public async Task<int> CreateAsync(UserCurrentWeather userCurrentWeather)
        {
            await _apiDbContext.userCurrentWeather.AddAsync(userCurrentWeather);
            return await _apiDbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<CurrentWeather>> GetCurrentWeatherNyUserid(string userid)
        {
          return  await _apiDbContext.userCurrentWeather
                        .Where(q => q.UserId == userid)
                        .Select(q => q.currentWeather).Take(8)
                        .OrderByDescending(q=>q.Id).ToListAsync();
        }
    }
}
