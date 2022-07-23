namespace WeatherForecastService.Dtos.WeatherForecast
{
    public class ForecastReadDto
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string Icon { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
    }
}
