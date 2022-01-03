using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace PrometheusMetrics
{
    public class MetricsFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly ILogger<MetricsFilterAttribute> _log;

        public MetricsFilterAttribute(ILogger<MetricsFilterAttribute> log) => _log = log;

        internal static void CallBackForMyObservation(Int64ObserverMetric observerMetric)
        {
            var labels1 = new List<KeyValuePair<string, string>> { new("dim1", "value1") };

            observerMetric.Observe(Process.GetCurrentProcess().WorkingSet64, labels1);
        }

        private static async Task CollectMetrics()
        {

            var meterProvider = MeterProvider.Default;
            var meter = meterProvider.GetMeter("MyMeter");

            // the rest is purely from Metrics API.
            var testCounter = meter.CreateInt64Counter("MyCounter");
            var testMeasure = meter.CreateInt64Measure("MyMeasure");
            var testObserver = meter.CreateInt64Observer("MyObservation", CallBackForMyObservation);
            var labels1 = new List<KeyValuePair<string, string>> { new("dim1", "value1") };

            var labels2 = new List<KeyValuePair<string, string>> { new("dim1", "value2") };
            var defaultContext = default(SpanContext);

            Stopwatch sw = Stopwatch.StartNew();
            while (sw.Elapsed.Seconds < 5)
            {
                testCounter.Add(defaultContext, 100, meter.GetLabelSet(labels1));

                testMeasure.Record(defaultContext, 100, meter.GetLabelSet(labels1));
                testMeasure.Record(defaultContext, 500, meter.GetLabelSet(labels1));
                testMeasure.Record(defaultContext, 5, meter.GetLabelSet(labels1));
                testMeasure.Record(defaultContext, 750, meter.GetLabelSet(labels1));

                // Obviously there is no testObserver.Oberve() here, as Observer instruments
                // have callbacks that are called by the Meter automatically at each collection interval.

                await Task.Delay(1000);
                var remaining = (1 * 60) - sw.Elapsed.TotalSeconds;
                Console.WriteLine("Running and emitting metrics. Remaining time:" + (int)remaining + " seconds");
            }

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            try
            {
                await CollectMetrics();
            }
            catch (Exception e)
            {
                _log.LogError(e.StackTrace);
            }
            finally
            {
                await Task.Delay(100);

                await next();
            }
        }
    }
}