using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RequestChain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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