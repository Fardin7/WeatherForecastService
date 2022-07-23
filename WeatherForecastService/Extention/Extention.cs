using WeatherForecastService.Dtos.Api;
using WeatherForecastService.Dtos.WeatherForecast;

namespace WeatherForecastService.Extention
{
    public static class Extention
    {
        const int NumberOfThreeHours = 8;
        public static IEnumerable<ForecastReadDto> WeatherForecastListComputation(this WeatherForecastList weatherForecastList)
        {
            List<ForecastReadDto> weathers = new List<ForecastReadDto>();

            var forecastData = weatherForecastList.list;
            while (forecastData.Any())
            {
                var day = forecastData.Take(NumberOfThreeHours).ToList();
                weathers.Add(new ForecastReadDto()
                {
                    Humidity = Math.Round(day.Select(q => q.main.humidity).Average()),
                    Temperature = Math.Round(day.Select(q => q.main.temp).Average()),
                    WindSpeed = Math.Round(day.Select(q => q.wind.speed).Average()),
                    Icon = day[new Random().Next(8)].weather[0].icon,
                    Description = day[new Random().Next(8)].weather[0].description
                });
                forecastData = forecastData.Skip(NumberOfThreeHours).ToList();
            }

            return weathers;
        }
        public static ForecastReadDto WeatherForecastComputation(this WeatherForecast weatherForecast)
        {
            ForecastReadDto result = new ForecastReadDto();

            result.Humidity = Math.Round(weatherForecast.main.humidity);
            result.Temperature = Math.Round(weatherForecast.main.temp);
            result.WindSpeed = Math.Round(weatherForecast.wind.speed);
            result.Icon = weatherForecast.weather[0].icon;
            result.City = weatherForecast.name;

            return result;
        }
    }
}
