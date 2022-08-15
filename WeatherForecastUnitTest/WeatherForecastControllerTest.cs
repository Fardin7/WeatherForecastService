using Domain.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using WeatherForecastService.Api;
using WeatherForecastService.Controllers;
using Moq;
using WeatherForecastService.Dtos.WeatherForecast;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Domain.Models;

namespace WeatherForecastUnitTest
{
    public class WeatherForecastControllerTest
    {
        private ILogger<WeatherForecastController> _logger;
        private Mock<IWeatherForecastApi> _clientUpdate;
        private Mock<IUserCurrentWeatherRepository> _userCurrentWeatherRepository;
        private WeatherForecastController _weatherForecastController;
        public WeatherForecastControllerTest()
        {
            _logger = new NullLogger<WeatherForecastController>();
            _clientUpdate = new Mock<IWeatherForecastApi>();
            _userCurrentWeatherRepository = new Mock<IUserCurrentWeatherRepository>();
        }
        [Fact]
        public async Task GetNextFiveDays_ViewModelIsNotValid_BadRequest()
        {
            //Arrange
            _weatherForecastController = new WeatherForecastController(_logger, _clientUpdate.Object
                                                                    , _userCurrentWeatherRepository.Object);
            _weatherForecastController.ModelState.AddModelError("City", "City Name Can Not be Null");

            //Act
            var result = await _weatherForecastController.GetNextFiveDays(It.IsAny<ForecastQueryDto>());

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetNextFiveDays_ViewModelIsValid_ReturnWeatherForecastData()
        {
            //Arrange
            var expectedResult = new List<ForecastReadDto>()
            {
                new ForecastReadDto
                {
                    Temperature = 10,
                    Humidity = 20,
                    WindSpeed = 20,
                    Icon = "d.png",
                    City = "Berlin",
                    Description = "Description"
               }
            };

            _clientUpdate.Setup(x => x.NextFiveDays(It.IsAny<ForecastQueryDto>()))
                          .ReturnsAsync(expectedResult);
            _weatherForecastController = new WeatherForecastController(_logger, _clientUpdate.Object
                                                                  , _userCurrentWeatherRepository.Object);

            //Act
            var actionResult = await _weatherForecastController.GetNextFiveDays(It.IsAny<ForecastQueryDto>());

            //Assert
            var OkResult = Assert.IsType<OkObjectResult>(actionResult);
            var list = Assert.IsType<List<ForecastReadDto>>(OkResult.Value);
            Assert.Equal("Berlin", list.FirstOrDefault().City);
        }

        [Fact]
        public async Task GetCuurent_ViewModelINotValid_ReturnBadRequest()
        {
            //Arrange
            _weatherForecastController = new WeatherForecastController(_logger, _clientUpdate.Object
                                                      , _userCurrentWeatherRepository.Object);
            _weatherForecastController.ModelState.AddModelError("Error", "City Name is Null!");

            //Act
            var result = await _weatherForecastController.GetCuurent(It.IsAny<ForecastQueryDto>());

            //Assert
            Assert.IsType<BadRequestResult>(result);

        }

        [Fact]
        public async Task GetCuurent_ViewModelIsValid_ReturnCurrentWeatherForecastData()
        {
            //Arrange
            var expectedResult = new ForecastReadDto
            {
                Temperature = 10,
                Humidity = 20,
                WindSpeed = 20,
                Icon = "d.png",
                City = "Berlin",
                Description = "Description"
            };
            var query = new ForecastQueryDto() { City = "Berlin" };
            _clientUpdate.Setup(x => x.Current(query, It.IsAny<string>()))
                .ReturnsAsync(expectedResult);
            _weatherForecastController = new WeatherForecastController(_logger, _clientUpdate.Object
                                                      , _userCurrentWeatherRepository.Object);
            ClaimsPrincipal user = UserPrincipal();
            _weatherForecastController.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };


            //Act
            var result = await _weatherForecastController.GetCuurent(query);

            //Assert
            var returnedResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ForecastReadDto>(returnedResult.Value);
            Assert.Equal(expectedResult.City, value.City);

        }

        [Fact]
        public async Task GetUserCuurentWeather_CuurentUserid_ReturnWeatherForecastOfCuurentUserId()
        {
            //Arrange
            var expectedResult = new List<CurrentWeather>() {
             new CurrentWeather()
            {
                 City="Berlin",
                 Humidity=10,
                 Temperature=10,
            },
                new CurrentWeather()
            {
                 City="koln",
                 Humidity=10,
                 Temperature=10,
            }
            };

            _userCurrentWeatherRepository.Setup(x => x.GetCurrentWeatherNyUserid(It.IsAny<string>()))
                .ReturnsAsync(expectedResult);
            _weatherForecastController = new WeatherForecastController(_logger, _clientUpdate.Object
                                                     , _userCurrentWeatherRepository.Object);
            ClaimsPrincipal user = UserPrincipal();

            _weatherForecastController.ControllerContext.HttpContext=new DefaultHttpContext() { User = user };

            //Act
            var result = await _weatherForecastController.GetUserCuurentWeather();

            //Assert
            var returnedResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<CurrentWeather>>(returnedResult.Value);
            Assert.Equal(2, value.Count);
            Assert.Equal("Berlin", value.FirstOrDefault().City);

        }
        private static ClaimsPrincipal UserPrincipal()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserId", "userid"),

                    }, "mock"));
        }
    }
}