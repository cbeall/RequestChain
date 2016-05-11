using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RequestChain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestChain.Test
{
    public class RequestIdTestServer : IDisposable
    {
        private readonly TestServer _server;

        public RequestIdTestServer(RequestChainOptions options = null)
        {
            var webApplicationBuilder = TestServer.CreateBuilder(SiteConfiguration(), 
                    ConfigureApplication, 
                    services => ConfigureServices(services, options));

            _server = new TestServer(webApplicationBuilder);
        }

        public HttpClient CreateClient()
        {
            return _server.CreateClient();
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        private IConfiguration SiteConfiguration()
        {
            return new ConfigurationBuilder()
                .Build();
        }

        private void ConfigureServices(IServiceCollection services, RequestChainOptions options)
        {
            if (options == null)
            {
                options = new RequestChainOptions
                {
                    IncludeRequestDepth = true
                };
            }

            services.AddRequestChain(options);
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            app.UseRequestChain();

            app.Map("/id", a => a.Run(GetRequestId));
            app.Map("/depth", a => a.Run(GetRequestDepth));
            app.Map("/header", a => a.Run(GetRequestHeader));

            app.Run(async a =>
            {
                a.Response.StatusCode = 400;
                await a.Response.WriteAsync("Did not use a known test method.");
            });
        }

        private async Task GetRequestId(HttpContext context)
        {
            var requestId = context.GetRequestId();

            if (!requestId.Value.Equals(default(Guid)))
            {
                await context.Response.WriteAsync(requestId.Value.ToString());
                context.Response.StatusCode = 200;
            }
            else
            {
                context.Response.StatusCode = 500;
            }
        }

        private async Task GetRequestDepth(HttpContext context)
        {
            var requestId = context.GetRequestId();

            if (requestId.Depth.HasValue)
            {
                await context.Response.WriteAsync(requestId.Depth.Value.ToString());
                context.Response.StatusCode = 200;
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }

        private async Task GetRequestHeader(HttpContext context)
        {
            string requestheader = RequestChainOptions.DefaultRequestIdHeader;

            if (context.Request.Query.ContainsKey("requestheader"))
            {
                requestheader = context.Request.Query["requestheader"].First();
            }

            if (context.Request.Headers.ContainsKey(requestheader))
            {
                await context.Response.WriteAsync(context.Request.Headers[requestheader].First());
                context.Response.StatusCode = 200;
            }
            else
            {
                context.Response.StatusCode = 500;
            }
        }
    }
}
