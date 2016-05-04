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

        public async Task Invoke(HttpContext context, IRequestId requestId, RequestChainOptions options)
        {
            RequestId settableRequestId = requestId as RequestId;

            settableRequestId.SetRequestId(context.Request, options);

            DateTime start = DateTime.Now;

            string requestDepth = string.Empty;

            if (options.IncludeRequestDepth && settableRequestId.HasValidDepth)
            {
                requestDepth = $"(Depth {requestId.Depth}) ";
            }

            _logger.Log(options.RequestBeginEndLogLevel,
                "Begin request {0} {1}for {2} on {3}", 
                requestId.Value, requestDepth, context.Request.Path, context.Request.PathBase);
                
            await _next.Invoke(context);

            TimeSpan requestTime = DateTime.Now - start;

            string statusCodeStr = string.Empty;
            
            if (context.Response.StatusCode != default(int))
            {
                statusCodeStr = $"with status code {context.Response.StatusCode} ";
            }

            _logger.Log(options.RequestBeginEndLogLevel, "End request {0} {2}({1} ms)", 
                requestId.Value, requestTime.Milliseconds, statusCodeStr);
        }
    }
}
