using FluentAssertions;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            var result = await _fixture.DefaultClient.MakeRequestAndGetRequestId();

            result.Should()
                .NotBe(default(Guid));
        }

        [Fact]
        public async Task RequestDepth_OriginalRequest_Zero()
        {
            var result = await _fixture.DefaultClient.MakeRequestAndGetRequestDepth();

            result.Should()
                .Be(0);
        }
    }
}
