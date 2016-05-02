using Microsoft.Extensions.DependencyInjection;

namespace RequestChain.Configuration
{
    public interface IRequestChainBuilder
    {
        IServiceCollection Services { get; }
    }
}