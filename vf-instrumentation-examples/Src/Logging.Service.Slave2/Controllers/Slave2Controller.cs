using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Slave2.Entities;
using Slave2.Persistence;

namespace Slave2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Slave2Controller : ControllerBase
    {
        private readonly ILogger<Slave2Controller> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _client;
        private readonly Slave2DbContext _db;
        private readonly ServicesUrlsOptions _servicesUrls;

        public Slave2Controller(ILogger<Slave2Controller> logger, IHttpContextAccessor httpContextAccessor,
            HttpClient client, IOptions<ServicesUrlsOptions> servicesUrls, Slave2DbContext db)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _client = client;
            _db = db;
            _servicesUrls = servicesUrls.Value;
        }

        [HttpGet(Name = "slave2ToSlave2")]
        [Route("slave2-slave2-factorial/{number:int}")]
        public async Task<int> Factorial(int number)
        {
            var host = _httpContextAccessor.HttpContext?.Request.Host.Value;
            _logger.LogInformation($"----HOST: {host}----", Array.Empty<object>());
            if (number == 0)
                return 1;

            var pathToMain = $"{_servicesUrls.Slave2Url}/Slave2/slave2-slave2-factorial/{number - 1}";

            var ns = await _client.GetStringAsync(pathToMain);

            return number * int.Parse(ns);
        }

        [HttpGet(Name = "masterToSlave2")]
        [Route("master-slave2-factorial/{number:int}")]
        public async Task<int> Factorial2(int number)
        {
            var host = _httpContextAccessor.HttpContext?.Request.Host.Value;
            _logger.LogInformation($"----HOST: {host}----", Array.Empty<object>());
            if (number == 0)
                return 1;

            var pathToMain = $"{_servicesUrls.MasterUrl}/api/recursive-master-slave2-factorial/{number - 1}";

            var ns = await _client.GetStringAsync(pathToMain);

            return number * int.Parse(ns);
        }

        [HttpGet]
        [Route("data")]
        public async Task<List<Data>> GetData()
        {
            return await _db.Datas.Select(d=>new Data
            {
                Id = d.Id,
                Value = d.Value,
                SubData = d.SubData.Select(s=> new SubData
                {
                    Id = s.Id,
                    Value = s.Value
                }).ToList()
            }).ToListAsync();
        }
    }
}
