using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoggingPOC
{
    public class Worker : IHostedService
    {
        private Timer _timer;
        private IDisposable _scope;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
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

            using (_logger.BeginScope(state))
            {
                _logger.LogTrace(message, 1, "This", new CustomObject());
                _logger.LogDebug(message, 2, "is", new CustomObject());
                _logger.LogInformation(message, 3, "a", new CustomObject());
                _logger.LogWarning(message, 4, "test", new CustomObject());
                _logger.LogError(message, 5, "of", new CustomObject());
                _logger.LogCritical(message, 6, "logging", new CustomObject());
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
