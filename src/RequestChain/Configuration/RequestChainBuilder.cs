using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestChain.Configuration
{
    public class RequestChainBuilder : IRequestChainBuilder
    {
        public RequestChainBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
