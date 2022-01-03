using Application.Common;
using MediatR;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Master.Data.Queries.GetSqlTraces
{
    public class GetSqlInstrumentationTracesQuery : IRequest<string>
    {
        internal class GetSqlInstrumentationTracesHandler : IRequestHandler<GetSqlInstrumentationTracesQuery, string>
        {
            private readonly ConnectionStrings _connectionStrings;

            public GetSqlInstrumentationTracesHandler(IOptions<ConnectionStrings> connectionStrings) =>
                _connectionStrings = connectionStrings.Value;

            public async Task<string> Handle(GetSqlInstrumentationTracesQuery request,
                CancellationToken cancellationToken) => await SqlServer(cancellationToken);

            private async Task<string> SqlServer(CancellationToken cancellationToken)
            {
                var res = string.Empty;

                await using var connection =
                    new SqlConnection(_connectionStrings.SqlServer);
                await connection.OpenAsync(cancellationToken);

                const string sql = "SELECT Value FROM data";

                await using var command = new SqlCommand(sql, connection);
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    res = reader.GetString(0);
                }

                return res;
            }
        }
    }
}
