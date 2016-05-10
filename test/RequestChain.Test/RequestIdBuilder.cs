using Mendham.Testing;
using RequestChain.Configuration;
using System;

namespace RequestChain.Test
{
    public class RequestIdBuilder : IBuilder<IRequestId>
    {
        private Guid _requestId;
        private int? _depth;
        private string _header;

        public RequestIdBuilder()
        {
            _requestId = Guid.NewGuid();
            _depth = DateTime.Now.Millisecond % 5 + 1;
            _header = RequestChainOptions.DefaultRequestIdHeader;
        }

        public RequestIdBuilder WithRequetId(Guid requestId)
        {
            _requestId = requestId;
            return this;
        }

        public RequestIdBuilder WithDepth(int depth)
        {
            _depth = depth;
            return this;
        }

        public RequestIdBuilder WithNoDepth()
        {
            _depth = null;
            return this;
        }

        public RequestIdBuilder WithHeader(string header)
        {
            _header = header;
            return this;
        }

        public IRequestId Build()
        {
            return new TestRequestId(_requestId, _depth, _header);
        }

        public class TestRequestId : IRequestId
        {
            public TestRequestId(Guid value, int? depth, string header)
            {
                Value = value;
                Depth = depth;
                RequestChainHeaderKey = header;
            }

            public Guid Value { get; }
            public int? Depth { get; }

            public string RequestChainHeaderKey { get; }

            public bool Equals(IRequestId other)
            {
                throw new NotImplementedException();
            }
        }
    }
}
