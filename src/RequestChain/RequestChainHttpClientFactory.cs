using Microsoft.Extensions.DependencyInjection;
using RequestChain.Configuration;
using System;
using System.Net.Http;

namespace RequestChain
{
    public class RequestChainHttpClientFactory : IRequestChainHttpClientFactory
    {
        private readonly RequestChainOptions _options;
        private readonly IRequestId _requestId;

        public RequestChainHttpClientFactory(IRequestId requestId, RequestChainOptions options)
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

        public RequestChainHttpClient CreateHttpClient()
        {
            return new RequestChainHttpClient(_requestId, _options);
        }

        public RequestChainHttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return new RequestChainHttpClient(_requestId, _options, handler);
        }

        public RequestChainHttpClient CreateHttpClient(HttpMessageHandler handler, bool disposeHandler)
        {
            return new RequestChainHttpClient(_requestId, _options, handler, disposeHandler);
        }
    }
}
