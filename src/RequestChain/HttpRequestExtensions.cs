using RequestChain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RequestChain
{
    public static class HttpRequestExtensions
    {
        public static void AddRequestChainHeader(this HttpRequestMessage request, IRequestId requestId, 
            RequestChainOptions options)
        {
            request.Headers.AddRequestChainHeader(requestId, options);
        }


        public static void AddRequestChainHeader(this HttpRequestHeaders headers, IRequestId requestId,
            RequestChainOptions options)
        {
            if (headers.Contains(options.RequestIdHeaderKey))
            {
                throw new InvalidOperationException("Attempted to set RequestChainHeader when it already exists");
            }

            string headerValue = requestId.Value.ToString();

            if (requestId.Depth.HasValue)
            {
                var newDepth = requestId.Depth.Value + 1;
                headerValue = $"{requestId.Value}:{newDepth}";
            }

            headers.Add(options.RequestIdHeaderKey, headerValue);
        }
    }
}
