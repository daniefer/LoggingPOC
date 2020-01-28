using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HttpClientLogging_Reworked
{
    public class RequestBodyLoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public RequestBodyLoggingHandler(ILogger<RequestBodyLoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content != null)
            {
                await request.Content.LoadIntoBufferAsync();
                var body = await request.Content.ReadAsStringAsync();
                _logger.LogInformation(body.Substring(0, 10));
                // reset stream so the caller can re-read the body;
                var stream = await request.Content.ReadAsStreamAsync();
                stream.Seek(0, SeekOrigin.Begin);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}