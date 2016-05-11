using FluentAssertions;
using RequestChain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RequestChain.Test
{
    public class RequestChainOptionsTests
    {
        [Fact]
        public async Task RequestId_FromCustomRequestHeader_RequestId()
        {
            var customRequestHeader = "custom-rc-header";
            var expectedId = Guid.NewGuid();

            var existingRequestId = new RequestIdBuilder()
                .WithRequetId(expectedId)
                .WithHeader(customRequestHeader)
                .Build();

            var options = new RequestChainOptions
            {
                RequestIdHeaderKey = customRequestHeader
            };

            using (var server = new RequestIdTestServer(options))
            using (var client = server.CreateClient())
            {
                client.ApplyRequestChain(existingRequestId);

                var result = await client.MakeRequestAndGetRequestId();

                result.Should()
                    .Be(expectedId, "the requestId comes from the existing request");
            }
        }

        [Fact]
        public async Task RequestDepth_FromCustomRequestHeader_RequestId()
        {
            var customRequestHeader = "custom-rc-header";

            var originalDepth = 4;
            var expectedDepth = 5;

            var existingRequestId = new RequestIdBuilder()
                .WithDepth(originalDepth)
                .WithHeader(customRequestHeader)
                .Build();

            var options = new RequestChainOptions
            {
                RequestIdHeaderKey = customRequestHeader
            };

            using (var server = new RequestIdTestServer(options))
            using (var client = server.CreateClient())
            {
                client.ApplyRequestChain(existingRequestId);

                var result = await client.MakeRequestAndGetRequestDepth();

                result.Should()
                    .Be(expectedDepth, "the original depth should increment when ApplyRequestChain is called on client");
            }
        }
    }
}
