using Domain.Interface;

namespace DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        ICurrentWeatherRepository currentWeatherRepository { get; }
        IUserCurrentWeatherRepository userCurrentWeatherRepository { get; }
        Task<int> CompleteAsync();

    }
}
