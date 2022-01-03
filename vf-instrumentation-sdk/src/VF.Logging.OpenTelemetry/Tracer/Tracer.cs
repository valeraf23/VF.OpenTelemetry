using System;
using System.Threading.Tasks;
using OpenTelemetry.Trace;

namespace VF.Logging.OpenTelemetry.VfTracer
{
    public class Tracer : ITracer
    {
        private const string DefaultDisplayName = "dotnet.custom.method.span";
        private readonly global::OpenTelemetry.Trace.Tracer _tracer;
        public Tracer(global::OpenTelemetry.Trace.Tracer tracer) => _tracer = tracer;
        public global::OpenTelemetry.Trace.Tracer GetTracer() => _tracer;
        public void AddSpan(Action act) => AddSpan(DefaultDisplayName, act);

        public void AddSpan(string spanName, Action act)
        {
            using var span = _tracer.StartSpan(spanName);
            act();
            using (global::OpenTelemetry.Trace.Tracer.WithSpan(span))
            {

            }
            span.End();
        }

        public async Task AddSpan(Func<Task> act) => await AddSpan(DefaultDisplayName, act);

        public async Task AddSpan(string spanName, Func<Task> act)
        {
            using var span = _tracer.StartSpan(spanName);
            await act();
            using (global::OpenTelemetry.Trace.Tracer.WithSpan(span))
            {

            }
            span.End();

        }
    }
}