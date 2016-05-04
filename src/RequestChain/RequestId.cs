﻿using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using RequestChain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RequestChain
{
    public class RequestId : IRequestId
    {
        private Guid _requestId;
        private int? _requestDepth;
        private static ILogger _logger;
        internal const char SplitCharacter = ':';

        internal RequestId()
        { }

        /// <summary>
        /// Unique request identifier derived by orignating request and passed to subsequent service calls.
        /// </summary>
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
        /// Depth of request call (0 is originating quest)
        /// </summary>
        public int? Depth
        {
            get
            {
                return _requestDepth;
            }
        }

        /// <summary>
        /// Determines if the depth is valid for display purposes
        /// </summary>
        internal bool HasValidDepth
        {
            get
            {
                return _requestDepth.HasValue && _requestDepth >= 0;
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
            KeyValuePair<string, StringValues> existingRequestIdHeader = request.Headers
                .FirstOrDefault(a => string.Equals(a.Key, options.RequestIdHeaderKey, StringComparison.OrdinalIgnoreCase));

            if (Equals(existingRequestIdHeader, default(KeyValuePair<string, StringValues>)))
            {
                // No request id set... creating a new one.

                // Sets value in object
                _requestId = Guid.NewGuid();
                _requestDepth = 0;

                // Add it to the request header
                request.Headers.Add(options.RequestIdHeaderKey, $"{_requestId}:0");

                _logger?.Log(options.RequestIdCreatedLogLevel, "RequestId created: {0}", _requestId);

                return false;
            }

            string requestIdStr = existingRequestIdHeader.Value.FirstOrDefault();

            if (!Guid.TryParse(requestIdStr.Split(SplitCharacter)[0], out _requestId))
            {
                if (options.ThrowOnMalformedRequestHeaders)
                {
                    throw new InvalidOperationException($"RequestId in header was not in correct format: \"{requestIdStr}\"");
                }
                else
                {
                    _requestId = Guid.NewGuid();
                    _requestDepth = 0;

                    _logger?.Log(options.RequestIdHeaderMalformedLogLevel, 
                        "RequestId in header was not in correct format \"{0}\". Replacing with new RequestId: {1}",
                        requestIdStr, _requestId);

                    return false;
                }
            }
            else
            {
                _requestDepth = GetRequestDepth(requestIdStr, options);
            }

            return true;
        }

        private int? GetRequestDepth(string requestIdString, RequestChainOptions options)
        {
            if (!options.IncludeRequestDepth)
            {
                return null;
            }

            var requestDepthStr = requestIdString
                .Split(SplitCharacter)
                .Skip(1)
                .FirstOrDefault();

            int requestDepth;
            
            if (string.IsNullOrEmpty(requestDepthStr) || !int.TryParse(requestDepthStr, out requestDepth) || requestDepth <= 0)
            {
                var msg = $"Request Depth for known RequestId {_requestId} was not found or malformed";

                if (options.ThrowOnMalformedRequestHeaders)
                {
                    throw new InvalidOperationException(msg);
                }
                else
                {
                    _logger?.Log(options.RequestIdHeaderMalformedLogLevel, msg);
                    return null;
                }
            }

            return requestDepth;
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
