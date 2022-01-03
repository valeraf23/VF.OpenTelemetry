using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Master.Orders.Queries.GetSqlTraces
{
    public class
        GetSqlTracesQueryHandler : IRequestHandler<GetSqlTracesQuery, string>
    {
        private readonly ConnectionStrings _connectionStrings;

        public GetSqlTracesQueryHandler(IOptions<ConnectionStrings> connectionStrings) => _connectionStrings = connectionStrings.Value;

        public async Task<string> Handle(GetSqlTracesQuery request,
            CancellationToken cancellationToken)
        {
            var res = string.Empty;

            await using var connection =
                new SqlConnection(_connectionStrings.SqlServer);
            await connection.OpenAsync(cancellationToken);

            const string sql = "SELECT Title FROM dbo.Orders";

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
