using Microsoft.Extensions.Logging;

namespace RequestChain.Configuration
{
    public class RequestChainOptions
    {
        private const string REQUESTID_DEFAULT_HEADER = "RC-RequestId";

        public string RequestIdHeaderKey { get; set; } = REQUESTID_DEFAULT_HEADER;

        public bool IncludeRequestDepth { get; set; } = true;
        public bool ThrowOnMalformedRequestHeaders { get; set; } = false;

        public LogLevel RequestBeginEndLogLevel { get; set; } = LogLevel.Information;
        public LogLevel RequestIdCreatedLogLevel { get; set; } = LogLevel.Information;
        public LogLevel RequestIdHeaderMalformedLogLevel { get; set; } = LogLevel.Warning;
    }
}
