using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HttpClientLogging_Reworked
{
    public class ResponseBodyLoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public ResponseBodyLoggingHandler(ILogger<ResponseBodyLoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (response.Content != null)
            {
                await response.Content.LoadIntoBufferAsync();
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(body.Substring(0, 10));
                // reset stream so the caller can re-read the body;
                var stream = await response.Content.ReadAsStreamAsync();
                stream.Seek(0, SeekOrigin.Begin);
            }
            return response;
        }
    }
}