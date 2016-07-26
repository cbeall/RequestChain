using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
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
        private HttpMessageHandler _backchannelHttpHandler;

        public RequestIdTestServer(RequestChainOptions options = null)
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureServices(services => ConfigureServices(services, options))
                .Configure(ConfigureApplication);

            _server = new TestServer(webHostBuilder);
        }

        public HttpClient CreateClient()
        {
            return _server.CreateClient();
        }

        public HttpMessageHandler CreateHandler()
        {
            return _server.CreateHandler();
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        public void SetBackchannelHttpHandler(HttpMessageHandler backchannelHttpHandler)
        {
            _backchannelHttpHandler = backchannelHttpHandler;
        }

        public IServiceProvider ServiceProvider
        {
            get { return _server.Host.Services; }
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
            app.Map("/backchannel-id", a => a.Run(GetRequestIdViaBackchannel));
            app.Map("/backchannel-depth", a => a.Run(GetRequestDepthViaBackchannel));

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
                context.Response.StatusCode = 204;
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

        private async Task GetRequestIdViaBackchannel(HttpContext context)
        {
            if (_backchannelHttpHandler == null)
            {
                throw new Exception("Backchannel HttpHandler is null");
            }

            var currentRequestId = context.GetRequestId();

            using (var client = new HttpClient(_backchannelHttpHandler) { BaseAddress = _server.BaseAddress })
            {
                var backchannelResponse = await client.GetAsync("id");

                backchannelResponse.EnsureSuccessStatusCode();

                var backchannelContent = await backchannelResponse.Content.ReadAsStringAsync();
                Guid backchannelRequestId;
                
                if (!Guid.TryParse(backchannelContent, out backchannelRequestId))
                {
                    // RequestId not returned from server
                    context.Response.StatusCode = 500;
                }
                else if (backchannelRequestId == currentRequestId.Value)
                {
                    // RequestId Match
                    context.Response.StatusCode = 200;
                }
                else
                {
                    // RequestId Mismatch
                    context.Response.StatusCode = 409;
                }
            }
        }

        private async Task GetRequestDepthViaBackchannel(HttpContext context)
        {
            if (_backchannelHttpHandler == null)
            {
                throw new Exception("Backchannel HttpHandler is null");
            }

            using (var client = new HttpClient(_backchannelHttpHandler) { BaseAddress = _server.BaseAddress })
            {
                var backchannelResponse = await client.GetAsync("depth");

                var backchannelContent = await backchannelResponse.Content.ReadAsStringAsync();

                context.Response.StatusCode = (int)backchannelResponse.StatusCode;
                await context.Response.WriteAsync(backchannelContent);
            }
        }
    }
}
