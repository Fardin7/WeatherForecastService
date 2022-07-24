namespace WeatherForecastService.Dtos.Config
{
    public class JwtConfig
    {
        public string Key { get; set; }
        public string ExpirationDays { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
