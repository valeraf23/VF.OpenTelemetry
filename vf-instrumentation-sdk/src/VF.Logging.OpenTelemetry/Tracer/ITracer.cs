using System;
using System.Threading.Tasks;
using OpenTelemetry.Trace;

namespace VF.Logging.OpenTelemetry.VfTracer
{
    public interface ITracer
    {
        global::OpenTelemetry.Trace.Tracer GetTracer();
        void AddSpan(Action act);
        void AddSpan(string spanName, Action act);
        Task AddSpan(Func<Task> act);
        Task AddSpan(string spanName, Func<Task> act);
    }
}