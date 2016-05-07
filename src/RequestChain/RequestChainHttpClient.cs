using RequestChain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace RequestChain
{
    public class RequestChainHttpClient : HttpClient
    {
        private readonly RequestChainOptions _options;
        private readonly IRequestId _requestId;

        internal RequestChainHttpClient(IRequestId requestId, RequestChainOptions options)
            : this(requestId, options, new HttpClientHandler(), true)
        { }

        internal RequestChainHttpClient(IRequestId requestId, RequestChainOptions options, HttpMessageHandler handler)
            : this(requestId, options, handler, true)
        { }

        internal RequestChainHttpClient(IRequestId requestId, RequestChainOptions options, HttpMessageHandler handler, bool disposeHandler)
            :base(handler, disposeHandler)
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

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.AddRequestChainHeader(_requestId, _options);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
