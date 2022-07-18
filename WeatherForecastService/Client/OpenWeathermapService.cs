using WeatherForecastService.Dtos;
using System.Text.Json;
using DataAccess;
using Domain.Models;

namespace WeatherForecastService.Client
{
    public class OpenWeathermapService : IClientUpdate
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        const int NumberOfThreeHours = 8;

        public OpenWeathermapService(IHttpClientFactory httpClientFactory, IConfiguration configuration
          , IUnitOfWork unitOfWork)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _unitOfWork = unitOfWork;


        }
        public async Task<IEnumerable<ForcastReadDto>> NextFiveDays(ForcastDto forcastDto, string userid)
        {
            ForcastDataDto? forcast = null;
            using (HttpClient _httpClient = _httpClientFactory.CreateClient("ServicePlicy"))
            {
                var httpResponse = await _httpClient.
                     GetAsync($"https://api.openweathermap.org/data/2.5/forecast?q={forcastDto.City}&appid={_configuration.GetSection("API Key").Value}");

                httpResponse.EnsureSuccessStatusCode();
                try
                {
                    var streamedResult = await httpResponse.Content.ReadAsStreamAsync();
                    forcast = await JsonSerializer.DeserializeAsync<ForcastDataDto>(streamedResult);
                    List<ForcastReadDto> weathers = new List<ForcastReadDto>();
                    var value = forcast.list;

                    while (value.Any())
                    {
                        var day = value.Take(NumberOfThreeHours).ToList();
                        weathers.Add(new ForcastReadDto()
                        {
                            Humidity = day.Select(q => q.main.humidity).Average(),
                            Temperature = day.Select(q => q.main.temp).Average(),
                            WindSpeed = day.Select(q => q.wind.speed).Average()
                        });
                        value = value.Skip(NumberOfThreeHours).ToList();
                    }
                    var saved = await _unitOfWork.currentWeatherRepository.CreateAsync(new CurrentWeather() { Humidity = weathers[0].Humidity, Temperature = 1 });
                    await _unitOfWork.CompleteAsync();

                    await _unitOfWork.userCurrentWeatherRepository.Create(new UserCurrentWeather() { UserId = userid, CurrentWeatherId = saved.Id });
                    int res = await _unitOfWork.CompleteAsync();

                    return weathers;
                }
                catch (Exception EX)
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }
            return null;
        }
    }
}

