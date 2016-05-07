using System.Net.Http;

namespace RequestChain
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Initalizes a new instance of <see cref="HttpClient"/>
        /// </summary>
        /// <param name="requestId">The current request Id for the service</param>
        /// <returns><see cref="HttpClient"/> with RequestChain header attached</returns>
        public static HttpClient CreateHttpClient(this IRequestId requestId)
        {
            return new HttpClient()
                .ApplyRequestChain(requestId);
        }

        /// <summary>
        /// Initalizes a new instance of <see cref="HttpClient"/> with a specific handler.
        /// </summary>
        /// <param name="requestId">The current request Id for the service</param>
        /// <param name="handler">The HTTP handler stack to use for sending requests</param>
        /// <returns><see cref="HttpClient"/> with RequestChain header attached</returns>
        public static HttpClient CreateHttpClient(this IRequestId requestId, HttpMessageHandler handler)
        {
            return new HttpClient(handler)
                .ApplyRequestChain(requestId);
        }

        /// <summary>
        /// Initalizes a new instance of <see cref="HttpClient"/> with a specific handler.
        /// </summary>
        /// <param name="requestId">The current request Id for the service</param>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> responsible for processing the HTTP response
        /// message.
        /// <param name="disposeHandler">true if the inner handler should be disposed of by Dispose(), false if 
        /// you intend to reuse the inner handler.</param>
        /// <returns><see cref="HttpClient"/> with RequestChain header attached</returns>
        public static HttpClient CreateHttpClient(this IRequestId requestId, HttpMessageHandler handler, bool disposeHandler)
        {
            return new HttpClient(handler, disposeHandler)
                .ApplyRequestChain(requestId);
        }

        /// <summary>
        /// Applies request chain header to DefaultRequestHeaders of HttpClient if it is not already attached.
        /// The depth will be incremented by one from the current depth.
        /// </summary>
        /// <param name="httpClient">Client to make outbound calls</param>
        /// <param name="requestId">The current request Id for the service</param>
        /// <returns><see cref="HttpClient"/> with RequestChain header attached</returns>
        public static HttpClient ApplyRequestChain(this HttpClient httpClient, IRequestId requestId)
        {
            httpClient.DefaultRequestHeaders.ApplyRequestChainHeader(requestId);
            return httpClient;
        }
    }
}
