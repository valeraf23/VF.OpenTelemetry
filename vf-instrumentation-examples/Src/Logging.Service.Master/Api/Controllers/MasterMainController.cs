using System.Threading.Tasks;
using Application.Master.Data.Command.AddKafkaMessage;
using Application.Master.Data.Queries.GetCustomTraces;
using Application.Master.Data.Queries.GetDataAll;
using Application.Master.Data.Queries.GetErrorTraces;
using Application.Master.Data.Queries.GetNPlus1Traces;
using Application.Master.Data.Queries.GetParallelTraces;
using Application.Master.Data.Queries.GetRecursiveTraces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Master.Controllers
{
    [ApiController]
    [Route("data")]
    public class MasterMainController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MasterMainController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetAll()
        {
            var data = await _mediator.Send(new GetAllQuery());
            return Ok(data);
        }

        [HttpGet]
        [Route("customTraces", Name = "GetCustomTraces")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetCustomTraces()
        {
            var res = await _mediator.Send(new GetCustomTraces());
            return Ok(res);
        }

        [HttpGet]
        [Route("custom/without-span", Name = "GetCustomWithOutSpan")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetCustomWithOutSpan()
        {
            var res = await _mediator.Send(new GetCustomWithOutSpan());
            return Ok(res);
        }

        [HttpGet]
        [Route("custom/with-span", Name = "GetCustomWithSpan")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetCustomWithSpan()
        {
            var res = await _mediator.Send(new GetCustomWithSpan());
            return Ok(res);
        }

        [HttpPost]
        [Route("kafka-traces", Name = "GetKafkaTraces")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateKafkaTraces(CreateKafkaInstrumentationTracesCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet]
        [Route("parallel", Name = "ParallelTraces")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ParallelTraces()
        {
            var allData = await _mediator.Send(new GetParallelTracesQuery());
            return Ok(allData);
        }

        [HttpGet]
        [Route("recursive", Name = "RecursiveTraces")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<int> RecursiveTraces(int counter = 2) =>
            await _mediator.Send(new GetRecursiveTracesQuery(counter));

        [HttpGet]
        [Route("error", Name = "ErrorTraces")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ErrorTraces()
        {
            var data = await _mediator.Send(new GetErrorTracesQuery());
            return Ok(data);
        }

        [HttpGet]
        [Route("sub", Name = "NPlus1Traces")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> NPlus1Traces()
        {
            var data = await _mediator.Send(new GetNPlus1TracesQuery());
            return Ok(data);
        }
    }
}