using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace VF.Logging.OpenTelemetry.MapPropagator
{
    public static class MessagingMapPropagator
    {
        private static readonly TextMapPropagator Propagator = new TraceContextPropagator();

        public static T ReceiveMessage<T>(ActivitySource activitySource, Func<T, string, IEnumerable<string>> getter, Func<T> receiver, string activityName, Action<Activity?, T> addTags)
        {
            var message = receiver();

            var parentContext = Propagator.Extract(default, message, getter);
            Baggage.Current = parentContext.Baggage;

            using var activity = activitySource.StartActivity(activityName, ActivityKind.Consumer, parentContext.ActivityContext);
            addTags(activity, message);
            return message;
        }

        public static async Task<TResult> SendMessage<T, TResult>(ActivitySource activitySource, T message, Action<T, string, string> setter, Func<T, Task<TResult>> sender, string activityName, Action<Activity?,T> addTags)
        {
            using var activity = PropagatingContext(activitySource, message, setter, activityName);
            addTags(activity, message);
            return await sender(message);
        }

        public static void SendMessage<T>(ActivitySource activitySource, T message, Action<T, string, string> setter, Action<T> sender, string activityName, Action<Activity?,T> addTags)
        {
            using var activity = PropagatingContext(activitySource, message, setter, activityName);
            addTags(activity, message);
            sender(message);
        }

        private static Activity? PropagatingContext<T>(ActivitySource activitySource, T message, Action<T, string, string> setter, string activityName)
        {
            var activity = activitySource.StartActivity(activityName, ActivityKind.Producer);
            ActivityContext contextToInject = default;
            if (activity is not null)
                contextToInject = activity.Context;
            else if (Activity.Current is not null) contextToInject = Activity.Current.Context;

            Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), message, setter);

            return activity;
        }
    }
}