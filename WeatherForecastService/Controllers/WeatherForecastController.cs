using WeatherForecastService.Client;
using Microsoft.AspNetCore.Mvc;
using WeatherForecastService.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace WeatherForecastService.Controllers
{
    [ApiController]
    [Route("api/Weather")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IClientUpdate _clientUpdate;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IClientUpdate clientUpdate)
        {
            _logger = logger;
            _clientUpdate = clientUpdate;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ForcastDto forcastDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(await _clientUpdate.NextFiveDays(forcastDto, "a29b36e7-b405-49a3-901e-58dbfd168a66"));
        }
    }
}