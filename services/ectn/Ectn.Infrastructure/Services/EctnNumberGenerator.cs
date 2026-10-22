using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ectn.Application.Interfaces;
using Ectn.Infrastructure.Persistence;

namespace Ectn.Infrastructure.Services;

public class EctnNumberGenerator : IEctnNumberGenerator
{
    private readonly EctnDbContext _dbContext;

    public EctnNumberGenerator(EctnDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateOfficialNumberAsync(Guid tenantId, string portOfLoading, DateTime issueDate)
    {
        // Enterprise ECTN number generation: ECTN-{YEAR}-{PORT}-{SEQUENCE}
        // Utilizing a database sequence for strict transaction-safe collision prevention
        var year = issueDate.Year;

        // A03: the port code is client supplied, so it is reduced to alphanumerics before it is
        // ever used - it must never carry quotes or SQL fragments into downstream storage.
        var rawPort = portOfLoading?.Trim() ?? string.Empty;
        var portCode = new string(rawPort.ToUpperInvariant()
            .Take(3)
            .Where(char.IsLetterOrDigit)
            .ToArray());
        if (portCode.Length == 0) portCode = "GEN";

        long sequenceValue;
        using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
        {
            // A03: the sequence name is bound as a parameter instead of being interpolated into
            // the SQL statement, so no caller controlled value can reach the query text.
            command.CommandText = "SELECT nextval(@sequenceName::regclass)";
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@sequenceName";
            parameter.Value = $"ectn_seq_{tenantId:N}";
            command.Parameters.Add(parameter);

            _dbContext.Database.OpenConnection();
            var result = await command.ExecuteScalarAsync();
            sequenceValue = result != DBNull.Value ? Convert.ToInt64(result) : throw new InvalidOperationException("Sequence generation failed");
        }

        return $"ECTN-{year}-{portCode}-{sequenceValue}";
    }
}
