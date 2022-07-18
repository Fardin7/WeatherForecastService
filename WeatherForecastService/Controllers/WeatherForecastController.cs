using WeatherForecastService.Client;
using Microsoft.AspNetCore.Mvc;
using WeatherForecastService.Dtos;

namespace WeatherForecastService.Controllers
{
    [ApiController]
    [Route("api/Weather")]
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

            return Ok(await _clientUpdate.NextFiveDays(forcastDto));
        }
    }
}