using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using RequestChain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestChain
{
    public class RequestId : IRequestId
    {
        private Guid _requestId;
        private static ILogger _logger;

        public Guid Value
        {
            get
            {
                if (_requestId.Equals(default(Guid)))
                {
                    throw new InvalidOperationException("RequestId was never set");
                }

                return _requestId;
            }
        }

        /// <summary>
        /// Sets the Request Id by either pulling it out of the header or creating a new one.
        /// </summary>
        /// <param name="request">Current request</param>
        /// <param name="options">RequestChainOptions</param>
        /// <returns>True if the value previously existing, false if it was newly assigned</returns>
        internal bool SetRequestId(HttpRequest request, RequestChainOptions options)
        {
            KeyValuePair<string, StringValues> existingRequestHeader = request.Headers
                .FirstOrDefault(a => string.Equals(a.Key, options.RequestHeaderKey, StringComparison.OrdinalIgnoreCase));

            if (Equals(existingRequestHeader, default(KeyValuePair<string, StringValues>)))
            {
                _requestId = Guid.NewGuid();
                _logger?.Log(options.RequestIdCreatedLogLevel, "RequestId created: {0}", _requestId);

                return false;
            }

            string requestIdStr = existingRequestHeader.Value.FirstOrDefault();

            if (!Guid.TryParse(requestIdStr, out _requestId))
            {
                if (options.ThrowOnMalformedRequestId)
                {
                    throw new InvalidOperationException($"RequestId in header was not in correct format: \"{requestIdStr}\"");
                }
                else
                {
                    _requestId = Guid.NewGuid();
                    _logger?.Log(options.RequestIdMalformedLogLevel, 
                        "RequestId in header was not in correct format \"{0}\". Replacing with new RequestId: {1}",
                        requestIdStr, _requestId);

                    return false;
                }
            }

            return true;
        }


        internal static void SetLogger(ILogger logger)
        {
            if (_logger != default(ILogger))
            {
                throw new InvalidOperationException("Logger for RequestId is already configured");
            }

            _logger = logger;
        }
    }
}
