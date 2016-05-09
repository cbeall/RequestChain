using System;

namespace RequestChain
{
    public interface IRequestId : IEquatable<IRequestId>
    {
        /// <summary>
        /// Unique request identifier derived by orignating request and passed to subsequent service calls.
        /// </summary>
        Guid Value { get; }

        /// <summary>
        /// Depth of request call (0 is originating quest)
        /// </summary>
        int? Depth { get; }

        /// <summary>
        /// The header key for RequestChain
        /// </summary>
        string RequestChainHeaderKey { get; }
    }
}