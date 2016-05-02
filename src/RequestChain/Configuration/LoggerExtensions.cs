using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestChain.Configuration
{
    public static class LoggerExtensions
    {
        private static readonly Func<object, Exception, string> _messageFormatter = MessageFormatter;

        internal static void Log(this ILogger logger, LogLevel level, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(level, 0, new FormattedLogValues(message, args), null, _messageFormatter);
        }

        private static string MessageFormatter(object state, Exception error)
        {
            return state.ToString();
        }
    }
}
