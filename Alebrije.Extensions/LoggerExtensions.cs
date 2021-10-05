using System;
using Microsoft.Extensions.Logging;

namespace Alebrije.Extensions
{
    public static class LoggerExtensions
    {
        public static void Verbose(this ILogger logger, string message, Exception exception = null)
        {
            logger.LogTrace(exception: exception, message: message);
        }

        public static void Debug(this ILogger logger, string message, Exception exception = null)
        {
            logger.LogDebug(exception: exception, message: message);
        }

        public static void Info(this ILogger logger, string message, Exception exception = null)
        {
            logger.LogInformation(exception: exception, message: message);
        }

        public static void Warning(this ILogger logger, string message, Exception exception = null)
        {
            logger.LogWarning(exception: exception, message: message);
        }

        public static void Error(this ILogger logger, string message, Exception exception = null)
        {
            logger.LogError(exception: exception, message: message);
        }

        public static void Fatal(this ILogger logger, string message, Exception exception = null)
        {
            logger.LogCritical(exception: exception, message: message);
        }
    }
}