using Domain.Interface;

namespace DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApiDbContext _apiDbContext;
        public ICurrentWeatherRepository currentWeatherRepository { get; }
        public IUserCurrentWeatherRepository userCurrentWeatherRepository { get; }
        public UnitOfWork(ApiDbContext apiDbContext,
            ICurrentWeatherRepository currentWeatherRepository,
            IUserCurrentWeatherRepository userCurrentWeatherRepository)
        {
            this._apiDbContext = apiDbContext;
            this.currentWeatherRepository = currentWeatherRepository;
            this.userCurrentWeatherRepository = userCurrentWeatherRepository;
        }
        public async Task<int> CompleteAsync()
        {
            return await _apiDbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _apiDbContext.Dispose();
            }
        }
    }
}
