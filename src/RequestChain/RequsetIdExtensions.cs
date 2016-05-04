using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
