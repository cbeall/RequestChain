using RequestChain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RequestChain.Handler
{
    public class RequestChainHandler : HttpMessageHandler
    {
        private readonly RequestChainOptions _options;
        private readonly IRequestId _requestId;
        private HttpMessageHandler _httpClientHandler;

        public RequestChainHandler(IRequestId requestId, RequestChainOptions options)
        {
            if (requestId == null)
            {
                throw new ArgumentNullException(nameof(requestId));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _requestId = requestId;
            _options = options;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add(_options.RequestIdHeaderKey, GenerateHeader());

            if (true)
            {
                var httpClientHandler = new InternalHandler();
                return httpClientHandler.SendAsync(request, cancellationToken);
            }

            _httpClientHandler = _httpClientHandler ?? new InternalHandler();

            return (_httpClientHandler)
        }

        internal void AddHandler(HttpMessageHandler handler)
        {
            if (_httpClientHandler != null)
            {
                throw new InvalidOperationException("HttpMessageHandler is already applied");
            }

            _httpClientHandler = handler;
        }

        private string GenerateHeader()
        {
            if (_requestId.Depth.HasValue)
            {
                var newDepth = _requestId.Depth.Value + 1;
                return $"{_requestId.Value}:{newDepth}";
            }
            else
            {
                return _requestId.Value.ToString();
            }
        }

        private class InternalHandler : HttpClientHandler
        {
            public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
