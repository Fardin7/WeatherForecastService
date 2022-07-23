namespace WeatherForecastService.Dtos.User
{
    public class RequestValidationDto
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}
