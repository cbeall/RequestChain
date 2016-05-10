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

        public RequestIdTestServer()
        {
            var webApplicationBuilder = TestServer.CreateBuilder(SiteConfiguration(), 
                    ConfigureApplication, 
                    services => ConfigureServices(services));

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

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddRequestChain(options =>
            {
                //options.RequestIdHeaderKey = CustomHeaderName;
                options.IncludeRequestDepth = true;
            });
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            app.UseRequestChain();

            app.Map("/id", a => a.Run(GetRequestId));
            app.Map("/depth", a => a.Run(GetRequestDepth));
            app.Map("/header", a => a.Run(GetRequestHeader));

            app.Run(a =>
            {
                a.Response.StatusCode = 404;
                return Task.FromResult(0);
            });

        }

        private async Task GetRequestId(HttpContext context)
        {
            await context.Response.WriteAsync(context.GetRequestId().Value.ToString());
            context.Response.StatusCode = 200;
        }

        private async Task GetRequestDepth(HttpContext context)
        {
            if (context.GetRequestId().Depth.HasValue)
            {
                await context.Response.WriteAsync(context.GetRequestId().Depth.Value.ToString());
                context.Response.StatusCode = 200;
            }

            context.Response.StatusCode = 404;
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
