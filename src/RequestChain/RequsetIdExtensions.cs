using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace RequestChain
{
    public static class RequsetIdExtensions
    {
        public static IRequestId GetRequestId(this HttpContext context)
        {
            return context.RequestServices.GetService<IRequestId>();
        }
    }
}
