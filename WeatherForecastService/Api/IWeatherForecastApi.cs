using WeatherForecastService.Dtos.WeatherForecast;

namespace WeatherForecastService.Api
{
    public interface IWeatherForecastApi
    {
        Task<IEnumerable<ForecastReadDto>> NextFiveDays(ForecastQueryDto forcastDto);
        Task<ForecastReadDto> Current(ForecastQueryDto forcastDto, string userid);
    }
}
