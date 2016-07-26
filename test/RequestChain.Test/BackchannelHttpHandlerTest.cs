using FluentAssertions;
using RequestChain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RequestChain.Test
{
    public class BackchannelHttpHandlerTest
    {
        [Fact]
        public async Task BackchannelHttpHandler_NextRequest_SameId()
        {
            using (var server = new RequestIdTestServer())
            {
                var serverHandler = server.CreateHandler();
                var backchannelHandler = server.ServiceProvider
                    .GetRequestChainBackchannelHttpHandler(serverHandler);

                server.SetBackchannelHttpHandler(backchannelHandler);

                using (var client = server.CreateClient())
                {
                    var result = await client.GetAsync("backchannel-id");

                    result.EnsureSuccessStatusCode();

                    result.StatusCode.Should()
                        .Be(HttpStatusCode.OK, "the endpoint returns ok when the request id matches");
                }
            }
        }

        [Fact]
        public async Task BackchannelHttpHandler_NextRequest_HasDepthOf2()
        {
            using (var server = new RequestIdTestServer())
            {
                var serverHandler = server.CreateHandler();
                var backchannelHandler = server.ServiceProvider
                    .GetRequestChainBackchannelHttpHandler(serverHandler);

                server.SetBackchannelHttpHandler(backchannelHandler);

                using (var client = server.CreateClient())
                {
                    var response = await client.GetAsync("backchannel-depth");

                    response.EnsureSuccessStatusCode();

                    response.StatusCode.Should()
                        .Be(HttpStatusCode.OK, "the endpoint returns ok when the request id matches");

                    var backchannelDepthStr = await response.Content.ReadAsStringAsync();
                    int backchannelDepth;

                    int.TryParse(backchannelDepthStr, out backchannelDepth).Should()
                        .BeTrue("the response content should be the depth");

                    backchannelDepth.Should()
                        .Be(1, "the backchannel is the second step in the process (the first is 0)");
                }
            }
        }
    }
}
