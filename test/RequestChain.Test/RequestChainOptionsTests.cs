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

        [Fact]
        public async Task RequestDepthExcluded_OriginalRequest_DepthNull()
        {
            var options = new RequestChainOptions
            {
                IncludeRequestDepth = false
            };

            using (var server = new RequestIdTestServer(options))
            using (var client = server.CreateClient())
            {
                var result = await client.MakeRequestAndGetRequestDepth();

                result.Should()
                    .Be(null, "the request depth is disabled in options");
            }
        }

        [Fact]
        public async Task RequestDepthExcluded_OriginalRequest_ValidRequestId()
        {
            var options = new RequestChainOptions
            {
                IncludeRequestDepth = false
            };

            using (var server = new RequestIdTestServer(options))
            using (var client = server.CreateClient())
            {
                var result = await client.MakeRequestAndGetRequestId();

                result.Should()
                    .NotBe(default(Guid), "it should be created by the server");
            }
        }

        [Fact]
        public async Task RequestDepthExcluded_OriginalRequest_HeaderWithRequestId()
        {
            var options = new RequestChainOptions
            {
                IncludeRequestDepth = false
            };

            Guid requestIdGuid;

            using (var server = new RequestIdTestServer(options))
            using (var client = server.CreateClient())
            {
                var result = await client.MakeRequestAndGetRequestHeader();

                result.Should()
                    .NotBeNullOrWhiteSpace();
                result.Should()
                    .NotContain(":", "the header only contains the request, not the depth");
                Guid.TryParse(result, out requestIdGuid)
                    .Should()
                    .BeTrue();
            }
        }

        [Fact]
        public async Task RequestDepthExcluded_ExistingRequestWithDepth_DepthNull()
        {
            var existingRequestId = new RequestIdBuilder()
                .WithDepth(3)
                .Build();

            var options = new RequestChainOptions
            {
                IncludeRequestDepth = false
            };

            using (var server = new RequestIdTestServer(options))
            using (var client = server.CreateClient())
            {
                client.ApplyRequestChain(existingRequestId);

                var result = await client.MakeRequestAndGetRequestDepth();

                result.Should()
                    .Be(null, "the request depth is disabled in options");
            }
        }

        [Fact]
        public async Task RequestDepthExcluded_ExistingRequestWithDepth_HeaderWithRequestId()
        {
            var existingRequestId = new RequestIdBuilder()
                .WithDepth(3)
                .Build();

            var options = new RequestChainOptions
            {
                IncludeRequestDepth = false
            };

            Guid requestIdGuid;

            using (var server = new RequestIdTestServer(options))
            using (var client = server.CreateClient())
            {
                client.ApplyRequestChain(existingRequestId);

                var result = await client.MakeRequestAndGetRequestHeader();

                result.Should()
                    .NotBeNullOrWhiteSpace();
                result.Should()
                    .NotContain(":", "the header only contains the request, not the depth");
                Guid.TryParse(result, out requestIdGuid)
                    .Should()
                    .BeTrue();
            }
        }

        [Fact]
        public async Task RequestDepthExcluded_ExistingRequestWithoutDepth_DepthNull()
        {
            var existingRequestId = new RequestIdBuilder()
                .WithNoDepth()
                .Build();

            var options = new RequestChainOptions
            {
                IncludeRequestDepth = false
            };

            using (var server = new RequestIdTestServer(options))
            using (var client = server.CreateClient())
            {
                client.ApplyRequestChain(existingRequestId);

                var result = await client.MakeRequestAndGetRequestDepth();

                result.Should()
                    .Be(null, "the request depth is disabled in options");
            }
        }

        [Fact]
        public async Task RequestDepthExcluded_ExistingRequestWithoutDepth_HeaderWithRequestId()
        {
            var existingRequestId = new RequestIdBuilder()
                .WithNoDepth()
                .Build();

            var options = new RequestChainOptions
            {
                IncludeRequestDepth = false
            };

            Guid requestIdGuid;

            using (var server = new RequestIdTestServer(options))
            using (var client = server.CreateClient())
            {
                client.ApplyRequestChain(existingRequestId);

                var result = await client.MakeRequestAndGetRequestHeader();

                result.Should()
                    .NotBeNullOrWhiteSpace();
                result.Should()
                    .NotContain(":", "the header only contains the request, not the depth");
                Guid.TryParse(result, out requestIdGuid)
                    .Should()
                    .BeTrue();
            }
        }
    }
}
