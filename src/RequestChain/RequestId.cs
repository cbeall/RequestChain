using Microsoft.AspNetCore.Http;
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
        private readonly RequestChainOptions _options;

        private static ILogger _logger;
        internal const char SplitCharacter = ':';

        internal RequestId(RequestChainOptions options)
        {
            if (options == default(RequestChainOptions))
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options;
        }


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
        /// The header key for RequestChain
        /// </summary>
        public string RequestChainHeaderKey
        {
            get
            {
                return _options.RequestIdHeaderKey;
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
                CreateNewRequestId(request, options);
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
                    CreateNewRequestId(request, options);

                    _logger?.Log(options.RequestIdHeaderMalformedLogLevel, 
                        "RequestId in header was not in correct format \"{0}\". Replacing with new RequestId: {1}",
                        requestIdStr, _requestId);

                    return false;
                }
            }
            else
            {
                _requestDepth = GetRequestDepth(requestIdStr, options, request.Headers);
            }

            return true;
        }

        private void CreateNewRequestId(HttpRequest request, RequestChainOptions options)
        {
            // Sets value in object
            _requestId = Guid.NewGuid();
            string requestHeaderValue;

            if (options.IncludeRequestDepth)
            {
                _requestDepth = 0;
                requestHeaderValue = $"{_requestId}:0";
            }
            else
            {
                _requestDepth = null;
                requestHeaderValue = _requestId.ToString();
            }

            // Add it to the request header which is important if request sent through reverse proxy
            request.Headers.Add(options.RequestIdHeaderKey, requestHeaderValue);
        }

        private int? GetRequestDepth(string requestIdString, RequestChainOptions options, IHeaderDictionary headers)
        {
            if (!options.IncludeRequestDepth)
            {
                requestIdString = RemoveDepthFromHeaderIfExists(requestIdString, options, headers);
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

        private string RemoveDepthFromHeaderIfExists(string requestIdString, RequestChainOptions options, IHeaderDictionary headers)
        {
            requestIdString = requestIdString
                .Split(SplitCharacter)
                .First();

            headers[options.RequestIdHeaderKey] = requestIdString;

            return requestIdString;
        }

        internal static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RequestId);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ Depth.GetHashCode();
        }

        public bool Equals(IRequestId other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.Equals(other.Value)
                && Equals(Depth, other.Depth);
        }
    }
}
