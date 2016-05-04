using Microsoft.Extensions.Logging;

namespace RequestChain.Configuration
{
    public class RequestChainOptions
    {
        public const string DefaultRequestIdHeader = "RC-RequestId";

        public string RequestIdHeaderKey { get; set; } = DefaultRequestIdHeader;

        public bool IncludeRequestDepth { get; set; } = true;
        public bool ThrowOnMalformedRequestHeaders { get; set; } = false;

        public LogLevel RequestBeginEndLogLevel { get; set; } = LogLevel.Information;
        public LogLevel RequestIdCreatedLogLevel { get; set; } = LogLevel.Information;
        public LogLevel RequestIdHeaderMalformedLogLevel { get; set; } = LogLevel.Warning;
    }
}
