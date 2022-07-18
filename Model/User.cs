using Microsoft.AspNetCore.Identity;
namespace Model
{
    public class User:IdentityUser
    {
        public ICollection<UserCurrentWeather> userCurrentWeather { get; set; }
    }
}
