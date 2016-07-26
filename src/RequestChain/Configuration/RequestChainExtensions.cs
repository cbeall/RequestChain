using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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
            services.AddSingleton(options);
            services.AddScoped<IRequestId, RequestId>(a => new RequestId(a.GetService<RequestChainOptions>()));

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<BackchannelHttpHandler>();

            return new RequestChainBuilder(services);
        }

        public static BackchannelHttpHandler GetRequestChainBackchannelHttpHandler(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<BackchannelHttpHandler>();
        }

        public static BackchannelHttpHandler GetRequestChainBackchannelHttpHandler(this IServiceProvider serviceProvider, HttpMessageHandler innerMessageHandler)
        {
            if (innerMessageHandler == default(HttpMessageHandler))
            {
                throw new ArgumentNullException(nameof(innerMessageHandler));
            }

            var httpAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            return new BackchannelHttpHandler(httpAccessor, innerMessageHandler);
        }
    }
}
