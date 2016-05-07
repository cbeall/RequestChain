using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestChain
{
    public interface IRequestChainHttpClientFactory
    {
        /// <summary>
        /// Initalizes a new instance of <see cref="RequestChainHttpClient"/>
        /// </summary>
        RequestChainHttpClient CreateHttpClient();

        /// <summary>
        /// Initalizes a new instance of <see cref="RequestChainHttpClient"/> with a specific handler.
        /// </summary>
        /// <param name="handler">The HTTP handler stack to use for sending requests</param>
        RequestChainHttpClient CreateHttpClient(HttpMessageHandler handler);

        /// <summary>
        /// Initalizes a new instance of <see cref="RequestChainHttpClient"/> with a specific handler.
        /// </summary>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> responsible for processing the HTTP response
        /// message.
        /// <param name="disposeHandler">true if the inner handler should be disposed of by Dispose(), false if 
        /// you intend to reuse the inner handler.</param>
        RequestChainHttpClient CreateHttpClient(HttpMessageHandler handler, bool disposeHandler);
    }
}
