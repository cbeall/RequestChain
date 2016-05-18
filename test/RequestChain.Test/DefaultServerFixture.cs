using System;
using System.Net.Http;

namespace RequestChain.Test
{
    public class DefaultServerFixture : IDisposable
    {
        private readonly RequestIdTestServer _server;

        public DefaultServerFixture()
        {
            _server = new RequestIdTestServer();
        }

        public HttpClient CreateClient()
        {
            return _server.CreateClient();
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}