using System;
using System.Threading.Tasks;
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
        var portCode = portOfLoading?.Substring(0, Math.Min(3, portOfLoading.Length)).ToUpperInvariant() ?? "GEN";
        
        long sequenceValue;
        using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = $"SELECT nextval('ectn_seq_{tenantId:N}')";
            _dbContext.Database.OpenConnection();
            var result = await command.ExecuteScalarAsync();
            sequenceValue = result != DBNull.Value ? Convert.ToInt64(result) : throw new Exception("Sequence generation failed");
        }

        return $"ECTN-{year}-{portCode}-{sequenceValue}";
    }
}
