using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RequestChain.Configuration
{
    public static class RequestChainExtensions
    {
        public static IApplicationBuilder UseRequestChain(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestChainMiddleware>();
        }

        public static IRequestChainBuilder AddRequestChain(this IServiceCollection services, Action<RequestChainOptions> setupAction = null)
        {
            var options = new RequestChainOptions();

            setupAction?.Invoke(options);

            return services.AddRequestChain(options);

        }

        public static IRequestChainBuilder AddRequestChain(this IServiceCollection services, RequestChainOptions options)
        {
            services.AddInstance(options);
            services.AddScoped<IRequestId, RequestId>(a => new RequestId(a.GetService<RequestChainOptions>()));

            return new RequestChainBuilder(services);
        }
    }
}
