using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace Journal.Infrastructure.Extensions
{
    public static class SeedExtensions
    {
        public static AsyncRetryPolicy CreatePolicy<T>(ILogger<T> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }

        public static string ReadDocumentFromExecutingAssembly(this string resourceName)
        {
            string data = "";
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                using StreamReader reader = new StreamReader(stream);
                data = reader.ReadToEnd();
            }
            catch (Exception)
            {
            }

            return data;
        }
    }
}
