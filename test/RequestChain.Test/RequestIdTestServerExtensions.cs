using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestChain.Test
{
    public static class RequestIdTestServerExtensions
    {
        public static async Task<Guid> MakeRequestAndGetRequestId(this HttpClient client)
        {
            return Guid.Parse(await client.GetStringAsync("id"));
        }

        public static async Task<int?> MakeRequestAndGetRequestDepth(this HttpClient client)
        {
            var result = await client.GetAsync("depth");

            if(!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Depth depth error ({result.StatusCode})");
            }

            var str = await result.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(str))
            {
                return int.Parse(str);
            }

            return null;
        }

        public static Task<string> MakeRequestAndGetRequestHeader(this HttpClient client, string customHeader = null)
        {
            if (customHeader == null)
            {
                return client.GetStringAsync("header");
            }
            else
            {
                return client.GetStringAsync($"header?requestheader={customHeader}");
            }
        }
    }
}
