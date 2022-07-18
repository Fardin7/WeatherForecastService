using Domain.Models;

namespace Domain.Interface
{
    public interface IUserCurrentWeatherRepository
    {
        Task Create(UserCurrentWeather userCurrentWeather);
    }
}
