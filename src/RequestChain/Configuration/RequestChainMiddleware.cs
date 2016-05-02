using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestChain.Configuration
{
    public class RequestChainMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestChainMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestChainMiddleware>();

            // Sets up static RequestId logger once
            RequestId.SetLogger(loggerFactory.CreateLogger<RequestId>());
        }

        public async Task Invoke(HttpContext context, RequestId requestId, RequestChainOptions options)
        {
            requestId.SetRequestId(context.Request, options);

            DateTime start = DateTime.Now;

            _logger.Log(options.RequestBeginEndLogLevel,
                "Begin request {0} for {1}", requestId.Value, context.Request.Path);
                
            await _next.Invoke(context);

            TimeSpan requestTime = DateTime.Now - start;

            _logger.Log(options.RequestBeginEndLogLevel, "End request {0} ({1} ms)", 
                requestId.Value, requestTime.Milliseconds);
        }

    }
}
