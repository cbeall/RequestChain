using System;

namespace RequestChain
{
    public interface IRequestId
    {
        Guid Value { get; }
    }
}