using Application.Metrics.Queries.GetMetrics;
using Application.Slave1.Queries.ParallelGet;
using Application.Slave1.Queries.SimpleGet;
using Application.Slave2.Queries.Recursive.MasterToSlave;
using Application.Slave2.Queries.Recursive.SlaveToSlave;
using Application.Slave2.Queries.SimpleGet;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetrySNPDemo.ApmCustomTraces;
using System.Net.Http;
using System.Threading.Tasks;
using Application.Master.Orders.Queries.GetOrdersList;
using Application.Master.Orders.Queries.GetSqlTraces;
using Application.Master.Orders.Queries.ParallelToServices;

namespace OpenTelemetrySNPDemo.Controllers
{
    [ApiController]
    [Route("api")]
    public class MasterController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly ILogger<MasterController> _logger;
        private readonly IMediator _mediator;

        public MasterController(HttpClient client, ILogger<MasterController> logger, IMediator mediator)
        {
            _client = client;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("sql-traces", Name = "GetSqlTraces")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetSqlTraces()
        {
            var res = await _mediator.Send(new GetSqlTracesQuery());
            return Ok(res);
        }

        [HttpGet]
        [Route("prometheus-metrics", Name = "GetPrometheusMetrics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetPrometheusMetrics()
        {
            var res = await _mediator.Send(new GetMetrics());
            return Ok(res);
        }

        [HttpGet]
        [Route("apm-custom-trace", Name = "GetApmAgentTrace")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public Task<IActionResult> GetApmAgentTrace()
        {
            return CustomTraces.ApmAgentTrace(async () =>
            {
                _ = await _client.GetStringAsync("http://google.com");
                return Ok("Get ApmAgent Trace");
            });
        }

        [HttpGet]
        [Route("all", Name = "GetOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetOrders()
        {
            var ol = await _mediator.Send(new GetOrdersList());
            return Ok(ol);

        }

        [HttpGet]
        [Route("slave1", Name = "GetOrdersFromSlave1")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetOrdersFromSlave1()
        {
            var ol = await _mediator.Send(new GetSlave1OrdersList());
            return Ok(ol);

        }

        [HttpGet]
        [Route("slave1/parallel/{buyer}", Name = "Parallel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Parallel(string buyer)
        {
            var ol = await _mediator.Send(new GetSlave1ParallelOrdersList(buyer));
            return Ok(ol);
        }

        [HttpGet]
        [Route("parallel-call-services", Name = "parallel2Services")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Parallel2Services()
        {
            var res = await _mediator.Send(new GetOrdersListFromParallel2Services());
            return Ok(res);
        }

        [HttpGet]
        [Route("slave2", Name = "GetOrdersFromSlave2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetOrdersFromSlave2()
        {
            var ol = await _mediator.Send(new GetSlave2OrdersList());
            return Ok(ol);

        }

        [HttpGet]
        [Route("recursive-slave2-slave2-factorial/{number}", Name = "Factorial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Recursive(int number)
        {
            var ol = await _mediator.Send(new GetSlave2ToSlave2Recursive(number));
            return Ok(ol);
        }

        [HttpGet]
        [Route("recursive-master-slave2-factorial/{number}", Name = "Factorial2MasterSlave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Recursive2(int number)
        {
            var ol = await _mediator.Send(new GetMasterToSlave2Recursive(number));
            return Ok(ol);
        }
    }
}