using WeatherForecastService.Api;
using Microsoft.AspNetCore.Mvc;
using WeatherForecastService.Dtos.WeatherForecast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Domain.Interface;

namespace WeatherForecastService.Controllers
{
    [ApiController]
    [Route("api/Weather")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastApi _clientUpdate;
        private readonly IUserCurrentWeatherRepository _userCurrentWeatherRepository;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, 
            IWeatherForecastApi clientUpdate,
            IUserCurrentWeatherRepository userCurrentWeatherRepository)
        {
            _logger = logger;
            _clientUpdate = clientUpdate;
            _userCurrentWeatherRepository = userCurrentWeatherRepository;
        }

        [HttpGet("GetNextFiveDays")]
        public async Task<IActionResult> GetNextFiveDays([FromQuery] ForecastQueryDto forcastDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(await _clientUpdate.NextFiveDays(forcastDto));
        }

        [HttpGet("GetCuurent")]
        public async Task<IActionResult> GetCuurent([FromQuery] ForecastQueryDto forcastDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var currentForecast = await _clientUpdate.Current(forcastDto, GetUserId());

            return Ok(currentForecast);
        }

        [HttpGet("GetUserCuurentWeather")]
        public async Task<IActionResult> GetUserCuurentWeather()
        {
            var lastForecasts = await _userCurrentWeatherRepository.GetCurrentWeatherNyUserid(GetUserId());

            return Ok(lastForecasts);
        }

        [NonAction]
        public string GetUserId()
        {
            return User.FindFirstValue("UserId");
        }
    }
}  