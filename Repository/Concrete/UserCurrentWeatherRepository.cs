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

        public UserCurrentWeatherRepository(ApiDbContext apiDbContext, IConfiguration configuration)
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
            var ListCount = Convert.ToInt32(_configuration.GetSection("HistoryListCount").Value);

            return await _apiDbContext.userCurrentWeather
                          .Where(q => q.UserId == userid)
                          .Select(q => q.currentWeather).Take(ListCount)
                          .OrderByDescending(q => q.Id).ToListAsync();
        }
    }
}
  