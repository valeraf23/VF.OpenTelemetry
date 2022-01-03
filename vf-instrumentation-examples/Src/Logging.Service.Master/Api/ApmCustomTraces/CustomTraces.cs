using System;
using System.Threading.Tasks;
using Elastic.Apm;
using Elastic.Apm.Api;
using Microsoft.AspNetCore.Mvc;

namespace Master.ApmCustomTraces
{
    public class CustomTraces
    {
        public static Task<IActionResult> ApmAgentTrace(Func<Task<IActionResult>> action)
        {
            var transaction = Agent
                .Tracer.StartTransaction("MyTransaction", ApiConstants.TypeRequest);
            var span = transaction.StartSpan("Select FROM customer",
                ApiConstants.TypeDb, ApiConstants.SubtypeMssql, ApiConstants.ActionQuery);

            var transaction2 = Agent
                .Tracer.StartTransaction("MyTransaction2", ApiConstants.TypeRequest,
                    DistributedTracingData.TryDeserializeFromString(transaction.OutgoingDistributedTracingData
                        .SerializeToString()));
            try
            {
                return action();
            }
            catch (Exception e)
            {
                transaction.CaptureException(e);
                throw;
            }
            finally
            {
                transaction.SetLabel("stringSample", "bar");
                transaction.SetLabel("boolSample", true);
                transaction.SetLabel("intSample", 42);
                span.End();
                transaction2.End();
                transaction.End();
            }
        }
    }
}
