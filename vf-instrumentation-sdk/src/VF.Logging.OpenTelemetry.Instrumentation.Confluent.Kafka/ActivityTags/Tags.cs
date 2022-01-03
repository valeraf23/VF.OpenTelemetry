using System.Collections.Generic;
using System.Linq;

namespace VF.Logging.OpenTelemetry.Instrumentation.Confluent.Kafka.ActivityTags
{
    public class Tags : ITags
    {
        private Tags(string[] values) => Values = values;

        public string[] Values { get; set; }

        public static ITags Create(IEnumerable<string> values) => new Tags(values.ToArray());
        public static ITags Create(string[] values) => new Tags(values);
    }
}