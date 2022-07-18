using WeatherForecastService.Dtos;

namespace WeatherForecastService.Client
{
    public interface IClientUpdate
    {
        Task<IEnumerable<ForcastReadDto>> NextFiveDays(ForcastDto forcastDto,string userid);
    }
}
