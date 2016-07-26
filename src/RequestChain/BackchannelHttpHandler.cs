using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace RequestChain
{
    public class BackchannelHttpHandler : HttpMessageHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpMessageInvoker _client;

        public BackchannelHttpHandler(IHttpContextAccessor httpContextAccessor)
            : this(httpContextAccessor, new HttpClientHandler())
        {
        }

        internal BackchannelHttpHandler(IHttpContextAccessor httpContextAccessor, HttpMessageHandler httpMessageHandler)
        {
            if (httpMessageHandler == default(HttpMessageHandler))
            {
                throw new ArgumentNullException(nameof(httpMessageHandler));
            }

            _httpContextAccessor = httpContextAccessor;
            _client = new HttpMessageInvoker(httpMessageHandler);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestId = _httpContextAccessor.HttpContext.GetRequestId();
            request.ApplyRequestChainHeader(requestId);

            return _client.SendAsync(request, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
