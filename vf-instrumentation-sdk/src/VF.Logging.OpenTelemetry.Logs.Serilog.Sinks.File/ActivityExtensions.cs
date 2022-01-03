using System.Diagnostics;

namespace VF.Logging.OpenTelemetry.Logs.Serilog.Sinks.File
{
    internal static class ActivityExtensions
    {
        public static string GetSpanId(this Activity activity)
        {

#if NETCOREAPP2_1

            var spanId = activity.Id;
            return spanId ?? string.Empty;
#endif

#if NETCOREAPP3_1_OR_GREATER

            var spanId = activity.IdFormat switch
            {
                ActivityIdFormat.Hierarchical => activity.Id,
                ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
                ActivityIdFormat.Unknown => null,
                _ => null,
            };

            return spanId ?? string.Empty;
#endif

        }

        public static string GetTraceId(this Activity activity)
        {
#if NETCOREAPP2_1

            var traceId = activity.RootId;
            return traceId ?? string.Empty;
#endif

#if NETCOREAPP3_1_OR_GREATER

            var traceId = activity.IdFormat switch
            {
                ActivityIdFormat.Hierarchical => activity.RootId,
                ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
                ActivityIdFormat.Unknown => null,
                _ => null,
            };

            return traceId ?? string.Empty;

#endif
        }

        public static string GetParentId(this Activity activity)
        {

#if NETCOREAPP2_1

            var parentId = activity.ParentId;
            return parentId ?? string.Empty;
#endif

#if NETCOREAPP3_1_OR_GREATER

            var parentId = activity.IdFormat switch
            {
                ActivityIdFormat.Hierarchical => activity.ParentId,
                ActivityIdFormat.W3C => activity.ParentSpanId.ToHexString(),
                ActivityIdFormat.Unknown => null,
                _ => null,
            };

            return parentId ?? string.Empty;

#endif
        }
    }
}