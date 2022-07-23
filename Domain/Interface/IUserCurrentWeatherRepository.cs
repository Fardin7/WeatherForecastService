using Domain.Models;

namespace Domain.Interface
{
    public interface IUserCurrentWeatherRepository
    {
        Task<int> CreateAsync(UserCurrentWeather userCurrentWeather);
        Task<IEnumerable<CurrentWeather>> GetCurrentWeatherNyUserid(string userid);
    }
}
