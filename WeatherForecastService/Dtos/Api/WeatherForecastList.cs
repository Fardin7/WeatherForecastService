namespace WeatherForecastService.Dtos.Api
{
    public class WeatherForecastList
    {
        public List<WeatherForecast> list { get; set; }
    }
    public class WeatherForecast
    {
        public Main main { get; set; }

        public Wind wind { get; set; }

        public List<Weather> weather { get; set; }

        public string name { get; set; }
    }
    public class Main
    {
        public double temp { get; set; }
        public double humidity { get; set; }
    }
    public class Wind
    {
        public double speed { get; set; }
    }
    public class Weather
    {
        public string icon { get; set; }
        public string description { get; set; }
    }
}
