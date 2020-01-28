using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HttpClientLogging_Reworked
{
    public class ResponseHeaderLoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public ResponseHeaderLoggingHandler(ILogger<ResponseHeaderLoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var sb = new StringBuilder();
            foreach (var header in response.Headers)
            {
                sb.Append(header.Key).Append(": ");
                foreach (var value in header.Value)
                {
                    sb.Append(value).Append(";");
                }
                sb.AppendLine();
            }
            _logger.LogInformation(sb.ToString());
            return response;
        }
    }
}