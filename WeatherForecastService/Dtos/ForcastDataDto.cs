namespace WeatherForecastService.Dtos
{
    public class ForcastDataDto
    {
        public List<ForcastData> list { get; set; }
    }
    public class ForcastData
    {
        public Main main { get; set; }

        public Wind wind { get; set; }
    }
    public class Main
    {
        public float temp { get; set; }
        public float humidity { get; set; }
    }
    public class Wind
    {
        public float speed { get; set; }
    }
}
