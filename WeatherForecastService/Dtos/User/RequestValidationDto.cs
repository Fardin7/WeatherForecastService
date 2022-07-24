namespace WeatherForecastService.Dtos.User
{
    public class RequestValidationDto
    {
        public string Token { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }
    }
}
