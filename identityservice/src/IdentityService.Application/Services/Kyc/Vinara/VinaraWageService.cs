using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Services.Kyc.Vinara;

public class VinaraWageService(IConfiguration configuration)
{
    public long RetrieveWage()
    {
        var wage = Convert.ToInt64(configuration["Vinara:Wage"]!);
        return wage;
    }
}