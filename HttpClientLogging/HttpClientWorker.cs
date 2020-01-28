using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttpClientLogging
{
    public class HttpClientWorker : IHostedService
    {
        private Timer _timer;
        private IDisposable _scope;
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientWorker(ILogger<HttpClientWorker> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Run, new { what = "is", the = "meaning", of = "this", state = 1 }, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5));
            _scope = _logger.BeginScope(new { _timer });
            _logger.LogTrace("starting...");
            return Task.CompletedTask;
        }

        private void Run(object state)
        {
            const string message = "{0} - Custom message: '{1}'. Custom Object: '{2}'";

            _logger.Log(LogLevel.Critical, "************{time}*************", DateTime.Now.TimeOfDay);

            var client = _httpClientFactory.CreateClient(nameof(HttpClientWorker));
            using (_logger.BeginScope(client))
            {
                var result = client.GetAsync("https://google.com").ConfigureAwait(false).GetAwaiter().GetResult();
                _logger.LogWarning(result.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult().Substring(0, 50).ToUpper() + "...");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
            _timer = null;
            _scope.Dispose();
            _scope = null;
            _logger.LogTrace("stopping...");
            return Task.CompletedTask;
        }
    }
}