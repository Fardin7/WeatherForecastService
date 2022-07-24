using System.Text.Json;
using DataAccess;
using Domain.Models;
using WeatherForecastService.Dtos.Api;
using WeatherForecastService.Extention;
using AutoMapper;
using Domain.Interface;
using Microsoft.Extensions.Options;
using WeatherForecastService.Dtos.Config;
using WeatherForecastService.Dtos.WeatherForecast;

namespace WeatherForecastService.Api
{
    public class WeatherForecastApi : IWeatherForecastApi
    {
        private readonly ICurrentWeatherRepository _currentWeatherRepository;
        private readonly IUserCurrentWeatherRepository _userCurrentWeatherRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly ApiConfig _apiConfig;
        public WeatherForecastApi(IHttpClientFactory httpClientFactory, IMapper mapper,
                                  ICurrentWeatherRepository currentWeatherRepository,
                                  IUserCurrentWeatherRepository userCurrentWeatherRepository,
                                  IOptionsMonitor<ApiConfig> apiConfigOptions)
        {
            _currentWeatherRepository = currentWeatherRepository;
            _userCurrentWeatherRepository = userCurrentWeatherRepository;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _apiConfig = apiConfigOptions.CurrentValue;
        }
        public async Task<IEnumerable<ForecastReadDto>> NextFiveDays(ForecastQueryDto forcastDto)
        {
            IEnumerable<ForecastReadDto>? result = null;

            using (HttpClient _httpClient = _httpClientFactory.CreateClient("ServicePlicy"))
            {
                var httpResponse = await _httpClient
                                         .GetAsync($"{_apiConfig.NextFiveDaysAPILink}{forcastDto.City}" +
                                          $"&appid={_apiConfig.APIKey}&units={_apiConfig.TemperatureUnit}");

                httpResponse.EnsureSuccessStatusCode();

                try
                {
                    var streamedResult = await httpResponse.Content.ReadAsStreamAsync();
                    var forcastData = await JsonSerializer.DeserializeAsync<WeatherForecastList>(streamedResult);

                    result = forcastData.WeatherForecastListComputation();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }
        public async Task<ForecastReadDto> Current(ForecastQueryDto forcastDto, string userid)
        {
            ForecastReadDto? result = null;

            using (HttpClient _httpClient = _httpClientFactory.CreateClient("ServicePlicy"))
            {
                var httpResponse = await _httpClient
                                         .GetAsync($"{_apiConfig.CurrentAPILink}{forcastDto.City}" +
                                          $"&appid={_apiConfig.APIKey}&units={_apiConfig.TemperatureUnit}");

                httpResponse.EnsureSuccessStatusCode();
                try
                {
                    var streamedResult = await httpResponse.Content.ReadAsStreamAsync();
                    var forcastData = await JsonSerializer.DeserializeAsync<WeatherForecast>(streamedResult);

                    result = forcastData.WeatherForecastComputation();

                    await InsertUserCurrentWeatherForecast(result, userid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }
        public async Task InsertUserCurrentWeatherForecast(ForecastReadDto forecastReadDto, string userid)
        {
            var savedWeatherForecast = await _currentWeatherRepository.
                               CreateAsync(_mapper.Map<CurrentWeather>(forecastReadDto));

            await _userCurrentWeatherRepository.
                     CreateAsync(new UserCurrentWeather() { UserId = userid, CurrentWeatherId = savedWeatherForecast.Id });
        }
    }
}

