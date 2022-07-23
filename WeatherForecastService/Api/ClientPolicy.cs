using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;

namespace WeatherForecastService.Api
{
    public class ClientPolicy
    {
        public AsyncRetryPolicy<HttpResponseMessage> immediateRetryPolicy { get; }
        public AsyncRetryPolicy<HttpResponseMessage> linearRetryPolicy { get; }
        public AsyncRetryPolicy<HttpResponseMessage> exponentialRetryPolicy { get; }
        public AsyncCircuitBreakerPolicy<HttpResponseMessage> circutBreakerPolicy { get; }

        public AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicyPolicy { get; }
        public ClientPolicy()
        {
            immediateRetryPolicy = Policy.HandleResult<HttpResponseMessage>(q => !q.IsSuccessStatusCode)
              .RetryAsync(3);

            linearRetryPolicy = Policy.HandleResult<HttpResponseMessage>(q => !q.IsSuccessStatusCode)
              .WaitAndRetryAsync(6, q => TimeSpan.FromSeconds(4));

            exponentialRetryPolicy = Policy.HandleResult<HttpResponseMessage>(q => !q.IsSuccessStatusCode)
              .WaitAndRetryAsync(6, retryattemp => TimeSpan.FromSeconds(Math.Pow(2, retryattemp)));

            circutBreakerPolicy = HttpPolicyExtensions
               .HandleTransientHttpError()
               .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10));

            timeoutPolicyPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(2));


        }
    }
}
