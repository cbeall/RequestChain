using System;

namespace RequestChain
{
    public interface IRequestId
    {
        /// <summary>
        /// Unique request identifier derived by orignating request and passed to subsequent service calls.
        /// </summary>
        Guid Value { get; }

        /// <summary>
        /// Depth of request call (0 is originating quest)
        /// </summary>
        int? Depth { get; }
    }
}