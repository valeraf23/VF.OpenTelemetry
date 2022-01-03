using Microsoft.AspNetCore.Mvc;

namespace PrometheusMetrics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrometheusMetricsController : ControllerBase
    {

        [HttpGet]
        [ServiceFilter(typeof(MetricsFilterAttribute))]
        public IActionResult Get() => Ok("Get Prometheus Metrics");
    }
}
