using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RequestChain.Test
{
    public class IncomingRequestTests : IClassFixture<DefaultServerFixture>
    {
        DefaultServerFixture _fixture;

        public IncomingRequestTests(DefaultServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task RequestId_OriginalRequest_HasValue()
        {
            using (var client = _fixture.CreateClient())
            {
                var result = await client.MakeRequestAndGetRequestId();

                result.Should()
                    .NotBe(default(Guid), "it should be created by the server");
            }
        }

        [Fact]
        public async Task RequestDepth_OriginalRequest_Zero()
        {
            using (var client = _fixture.CreateClient())
            {
                var result = await client.MakeRequestAndGetRequestDepth();

                result.Should()
                    .Be(0, "it is the original request which has depth 0");
            }
        }

        [Fact]
        public async Task RequestHeader_OriginalRequest_HeaderSetOnRequest()
        {
            Guid requestIdGuid;
            int requestIdDepth;

            using (var client = _fixture.CreateClient())
            {
                var result = await client.MakeRequestAndGetRequestHeader();

                result.Should()
                    .NotBeNullOrWhiteSpace();
                result.Split(':')
                    .Should()
                    .HaveCount(2);
                Guid.TryParse(result.Split(':')[0], out requestIdGuid)
                    .Should()
                    .BeTrue();
                int.TryParse(result.Split(':')[1], out requestIdDepth)
                    .Should()
                    .BeTrue();
                requestIdDepth.Should()
                    .Be(0);
            }
        }

        [Fact]
        public async Task RequestId_ExistingRequest_MaintainsId()
        {
            var expectedId = Guid.NewGuid();
            var existingRequestId = new RequestIdBuilder()
                .WithRequetId(expectedId)
                .Build();

            using (var client = _fixture.CreateClient())
            {
                client.ApplyRequestChain(existingRequestId);

                var result = await client.MakeRequestAndGetRequestId();

                result.Should()
                    .Be(expectedId, "the requestId comes from the existing request");
            }
        }
    }
}
