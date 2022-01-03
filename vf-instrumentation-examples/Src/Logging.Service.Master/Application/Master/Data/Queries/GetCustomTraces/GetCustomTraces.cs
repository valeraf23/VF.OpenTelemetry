using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using VF.Logging.OpenTelemetry.VfTracer;

namespace Application.Master.Data.Queries.GetCustomTraces
{
    public class GetCustomTraces : IRequest
    {
        internal class GetCustomTracesHandler : IRequestHandler<GetCustomTraces>
        {
            private readonly Tracer _tracer;
            private readonly DummyService _dummyService;
            private readonly ILogger<GetCustomTraces> _logger;

            public GetCustomTracesHandler(ILogger<GetCustomTraces> logger, IVfTracer tracer, DummyService dummyService)
            {
                _logger = logger;
                _tracer = tracer.GetTracer();
                _dummyService = dummyService;
            }

            public async Task<Unit> Handle(GetCustomTraces request, CancellationToken cancellationToken)
            {
                using var parentSpan = _tracer.StartActiveSpan("Master Custom Span");
                parentSpan.SetAttribute("Time", DateTime.Now.ToShortTimeString());

                await _dummyService.DoSomeWork();

                  var childSpan = _tracer.StartSpan("Master Custom Traces response post-processing");
                childSpan.AddEvent("start").SetAttribute("time", DateTime.Now.ToString(CultureInfo.InvariantCulture));

                childSpan.SetStatus(Status.Ok);
                childSpan.End();
                parentSpan.End();
                _logger.LogInformation(@"ConsoleApplication1.MyCustomException: some message .... ---> System.Exception: Oh noes!
   at ConsoleApplication1.SomeObject.OtherMethod() in C:\ConsoleApplication1\SomeObject.cs:line 24
   at ConsoleApplication1.SomeObject..ctor() in C:\ConsoleApplication1\SomeObject.cs:line 14
   --- End of inner exception stack trace ---
   at ConsoleApplication1.SomeObject..ctor() in C:\ConsoleApplication1\SomeObject.cs:line 18
   at ConsoleApplication1.Program.DoSomething() in C:\ConsoleApplication1\Program.cs:line 23
   at ConsoleApplication1.Program.Main(String[] args) in C:\ConsoleApplication1\Program.cs:line 13");
                return Unit.Value;
            }
        }
    }
}
