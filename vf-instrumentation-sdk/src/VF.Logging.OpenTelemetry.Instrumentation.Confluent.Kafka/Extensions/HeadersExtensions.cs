using System.Text;
using Confluent.Kafka;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.Extensions
{
    public static class HeadersExtensions
    {
        public static bool TryGetValue(this Headers headers, string key, out string value)
        {
            if (headers is not null)
            {
                foreach (var head in headers)
                {
                    if (!head.Key.Equals(key)) continue;
                    value = Encoding.UTF8.GetString(head.GetValueBytes());
                    return true;
                }
            }

            value = string.Empty;
            return false;
        }
    }
}