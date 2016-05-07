using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RequestChain
{
    public static class HttpRequestExtensions
    {
        public static void ApplyRequestChainHeader(this HttpRequestMessage request, IRequestId requestId)
        {
            request.Headers.ApplyRequestChainHeader(requestId);
        }

        public static void ApplyRequestChainHeader(this HttpRequestHeaders headers, IRequestId requestId)
        {
            string headerValue = requestId.Value.ToString();

            if (requestId.Depth.HasValue)
            {
                var newDepth = requestId.Depth.Value + 1;
                headerValue = $"{requestId.Value}:{newDepth}";
            }

            var existingHeader = headers
                .FirstOrDefault(a => string.Equals(a.Key, requestId.RequestChainHeaderKey));

            if (!Equals(existingHeader, default(KeyValuePair<string, IEnumerable<string>>)))
            {
                if (existingHeader.Value.Any(a => string.Equals(a, headerValue, StringComparison.OrdinalIgnoreCase)))
                {
                    // Header is already in place... exit adding process
                    return;
                }
                else
                {
                    var firstHeader = existingHeader.Value.FirstOrDefault();
                    var msg = $"Attempted to set RequestChainHeader when it already exists and does not match (\"{firstHeader}\")";
                    throw new InvalidOperationException(msg);
                }
            }

            headers.Add(requestId.RequestChainHeaderKey, headerValue);
        }
    }
}
