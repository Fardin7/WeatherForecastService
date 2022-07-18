namespace Domain.Models
{
    public class UserCurrentWeather
    {
        public int CurrentWeatherId { get; set; }

        public CurrentWeather currentWeather { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }
    }
}
