using WeatherForecastService.Dtos;
using System.Text;
using System.Text.Json;

namespace WeatherForecastService.Client
{
    public class OpenWeathermapService : IClientUpdate
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        const int NumberOfThreeHours = 8;

        public OpenWeathermapService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<IEnumerable<Weather>> NextFiveDays(ForcastDto forcastDto)
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
                    List<Weather> weathers = new List<Weather>();
                    var value = forcast.list;

                    while (value.Any())
                    {
                       var day= value.Take(NumberOfThreeHours).ToList();
                        weathers.Add(new Weather()
                        {
                            Humidity = day.Select(q => q.main.humidity).Average(),
                            Temperature = day.Select(q => q.main.temp).Average(),
                            WindSpeed = day.Select(q => q.wind.speed).Average()
                        });
                        value = value.Skip(NumberOfThreeHours).ToList();
                    }

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

