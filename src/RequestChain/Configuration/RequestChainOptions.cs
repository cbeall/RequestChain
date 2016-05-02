using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestChain.Configuration
{
    public class RequestChainOptions
    {
        private const string DEFAULT_REQUEST_HEADER = "RC-RequestId";

        public string RequestHeaderKey { get; set; } = DEFAULT_REQUEST_HEADER;

        public bool ThrowOnMalformedRequestId { get; set; } = false;

        public LogLevel RequestBeginEndLogLevel { get; set; } = LogLevel.Information;
        public LogLevel RequestIdCreatedLogLevel { get; set; } = LogLevel.Information;
        public LogLevel RequestIdMalformedLogLevel { get; set; } = LogLevel.Warning;
    }
}
