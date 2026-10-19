using IdentityService.Contracts.ChangeMobileNumber.Repositories;
using IdentityService.Domain.Entity.ChangeMobileNumber;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.ChangeMobileNumber;

public class ChangeMobileNumberRepository(ApplicationDBContext dbContext, IConfiguration configuration, ILogger<ChangeMobileNumberRepository> logger) : IChangeMobileNumberRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("IdentityConnection");
    public async Task<ChangeMobileNumberEntity> InsertAsync(ChangeMobileNumberEntity entity, CancellationToken cancellationToken)
    {
        var result = await dbContext.ChangeMobileNumber.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<ChangeMobileNumberEntity> RetrieveByTrackIdAsync(string trackId, CancellationToken cancellationToken)
    {
        var result = await dbContext.ChangeMobileNumber.FirstOrDefaultAsync(x => x.TrackId == trackId, cancellationToken);
        return result;
    }

    public async Task<bool> ChangeMobileNumbers(string oldMobileNumber, string newMobileNumber, string nationalCode, string referenceCode, EChangeMobileNumberStatus status, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            const string query = "[dbo].[UpdateIdentityMobileNumber]";
            await connection.OpenAsync(cancellationToken);
            _ = await connection.QueryFirstOrDefaultAsync(query,
                new
                {
                    OldNumber = oldMobileNumber,
                    NewNumber = newMobileNumber,
                    NationalCode = nationalCode,
                    ReferenceCode = referenceCode,
                    Status = status,
                },
                commandType: System.Data.CommandType.StoredProcedure);
            connection.Close();
            return true;
        }
        catch (Exception exp)
        {
            logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
            return false;
        }
    }
}