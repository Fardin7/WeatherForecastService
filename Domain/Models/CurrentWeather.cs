namespace Domain.Models
{
    public class CurrentWeather
    {
        public int Id { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public ICollection<UserCurrentWeather> userCurrentWeather { get; set; }
    }
}
