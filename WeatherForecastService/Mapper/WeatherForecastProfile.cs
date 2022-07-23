using AutoMapper;
using Domain.Models;
using WeatherForecastService.Dtos.WeatherForecast;

namespace WeatherForecastService.Mapper
{
    public class WeatherForecastProfile:Profile
    {
        public WeatherForecastProfile()
        {
            CreateMap<ForecastReadDto, CurrentWeather>();

        }
    }
}
