using System;
using System.Diagnostics;
using OpenTelemetry;

namespace Master
{
    internal class MyProcessor : BaseProcessor<Activity>
    {
        public override void OnStart(Activity activity)
        {
            Console.WriteLine($"OnStart: {activity.DisplayName}");
        }

        public override void OnEnd(Activity activity)
        {
            Console.WriteLine($"OnEnd: {activity.DisplayName}");
        }
    }
}