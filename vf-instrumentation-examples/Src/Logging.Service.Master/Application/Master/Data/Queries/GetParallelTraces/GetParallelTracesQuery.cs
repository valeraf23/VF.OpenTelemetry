using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Models;
using Application.Slave1;
using Application.Slave2;
using Confluent.Kafka;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Master.Data.Queries.GetParallelTraces
{
    public class GetParallelTracesQuery : IRequest<List<UserDto>>
    {
        internal class GetParallelTracesHandler : IRequestHandler<GetParallelTracesQuery, List<UserDto>>
        {
            private readonly IMasterDbContext _db;
            private readonly IProducer<Null, string> _messageSender;
            private readonly ISlave1Service _slave1;
            private readonly ISlave2Service _slave2;

            public GetParallelTracesHandler(IMasterDbContext db, ISlave1Service slave1, ISlave2Service slave2,
                IProducer<Null, string> messageSender)
            {
                _db = db;
                _slave1 = slave1;
                _slave2 = slave2;
                _messageSender = messageSender;
            }

            public async Task<List<UserDto>> Handle(GetParallelTracesQuery request, CancellationToken cancellationToken)
            {
                var master = GetFromMaster(cancellationToken);
                var getDataSlave1Task = _slave1.GetData();
                var getDataSlave2Task = _slave2.GetData();
                Task.WaitAll(new Task[] {master, getDataSlave1Task, getDataSlave2Task}, cancellationToken);
                await SendMsg("Parallel Call", cancellationToken);
                var data1 = await getDataSlave1Task;
                var data2 = await getDataSlave2Task;
                var data3 = await master;
                var allData = data1 + data2;
                var slaves = allData.Data.Select(data => new UserDto
                {
                    User = data.Value,
                    Message = data.SubData.Select(s => s.Value).ToList()
                }).ToList();
                slaves.AddRange(data3);
                return slaves;
            }

            private async Task SendMsg(string value, CancellationToken cancellationToken)
            {
                var text = $"{value} {DateTime.Now:g}";
                var message = new Message<Null, string> {Value = text};
                AddSizeHeader(10000, message);

                await _messageSender.ProduceAsync("vf.kafka_instrumentation_tracing", message, cancellationToken);
            }

            private static void AddSizeHeader(long size, MessageMetadata message)
            {
                var byteArray = new byte[size];
                var random = new Random();
                random.NextBytes(byteArray);
                message.Headers = new Headers {new Header("size", byteArray)};
            }

            private async Task<List<UserDto>> GetFromMaster(CancellationToken cancellationToken)
            {
                return await _db.User.Select(data => new UserDto
                {
                    User = data.Value,
                    Message = data.SubData.Select(s => s.Value).ToList()
                }).ToListAsync(cancellationToken);
            }
        }
    }
}