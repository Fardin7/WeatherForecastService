using Newtonsoft.Json;

namespace WeatherForecastService.Dtos
{
    public class Weather
    {
        [JsonProperty(PropertyName = "Temperature")]
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float WindSpeed { get; set; }
    }
}
