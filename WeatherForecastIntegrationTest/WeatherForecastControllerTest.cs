using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WeatherForecastService.Dtos.User;
using WeatherForecastService.Dtos.WeatherForecast;

namespace WeatherForecastIntegrationTest
{
    public class WeatherForecastControllerTest//: IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private HttpClient _client;

        public WeatherForecastControllerTest(/*WebApplicationFactory<Program> webApplicationFactory*/)
        {
            _webApplicationFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {

                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApiDbContext>));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }


                        services.AddDbContext<ApiDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryDbForTesting");
                        });
                        var sp = services.BuildServiceProvider();
                        using (var scope = sp.CreateScope())
                        using (var appContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>())
                        {
                            try
                            {
                                appContext.Database.EnsureCreated();
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }

                    });
                });
        }
        [Theory]
        [InlineData("Leipzig")]
        public async Task GetNextFiveDays_ViewModelIsValid_ReturnWeatherForecastData(string cityName)
        {
            //Arrange
            _client = _webApplicationFactory.CreateClient();
            await Authentication();

            //Act
            var returnedResult = await _client.GetAsync($"/api/Weather/GetNextFiveDays?City={cityName}");
            var streamedResult = await returnedResult.Content.ReadAsStreamAsync();

            var requestValidationDto = await JsonSerializer.DeserializeAsync<List<ForecastReadDto>>(streamedResult,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            //Assert
            var resultValue = Assert.IsType<List<ForecastReadDto>>(requestValidationDto);
            Assert.Equal(5, resultValue.Count);
            Assert.NotNull(resultValue.FirstOrDefault());

        }

        protected async Task Authentication()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwt());
        }

        private async Task<string?> GetJwt()
        {
            var userCreateDto = new UserCreateDto()
            {
                Email = "example@test3.com",
                Username = "Test3",
                Password = "Test*123564"
            };
            HttpContent content = new StringContent(JsonSerializer.Serialize(userCreateDto), Encoding.UTF8
                , "application/json");
            var result = await _client.PostAsync("/api/UserManagement/Register", content);

            var streamedResult = await result.Content.ReadAsStreamAsync();

            var requestValidationDto = await JsonSerializer.DeserializeAsync<RequestValidationDto>(streamedResult,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return requestValidationDto?.Token;

        }
    }
}