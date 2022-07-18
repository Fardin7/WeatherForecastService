using WeatherForecastService.Dtos;

namespace WeatherForecastService.Client
{
    public interface IClientUpdate
    {
        Task<IEnumerable<Weather>> NextFiveDays(ForcastDto forcastDto);
    }
}
