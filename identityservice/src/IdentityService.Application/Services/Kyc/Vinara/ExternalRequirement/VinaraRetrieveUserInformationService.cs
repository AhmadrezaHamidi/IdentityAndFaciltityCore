using System.Text.Json;
using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagementKycLog;
using IdentityService.Domain.IRepositories;
using Microsoft.Extensions.Logging;
using OmidBank.Common.Helper.Minio;

namespace IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement;

public class RetrieveUserInformationService(
    IUserManagementKycLogRepository userManagementKycLogRepository,
    IUserRepository userRepository,
    ImageStorageService imageStorageService,
    ILogger<RetrieveUserInformationService> logger
    )
{
    public async Task<VinaraBaseResponse<VinaraRetrieveUserInformationResponse>> Handle(VinaraRetrieveUserInformationRequest request, CancellationToken cancellationToken)
    {
        var umKycLog = await userManagementKycLogRepository.GetByKycTokenAsync(request.UniqueKey, cancellationToken);
        if (umKycLog is null)
            return MakeErrorResponse();
        var image = await imageStorageService.DownloadAsync(umKycLog.ImageName);
        if (image is null)
            return MakeErrorResponse();
        return await MakeSuccessResponse(umKycLog, image, cancellationToken);
    }
    
    private static async Task<string> StreamToBase64Async(Stream stream, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private async Task<VinaraBaseResponse<VinaraRetrieveUserInformationResponse>> MakeSuccessResponse(UserManagementKycLogEntity umKycLog, Stream image, CancellationToken cancellationToken)
    {
        var personId = await userRepository.GetPersonIdAsync(umKycLog.UserId, cancellationToken);
        logger.LogInformation("personId" + personId);
        var user = await userRepository.GetPersonByIdAsync(personId, cancellationToken);
        logger.LogInformation("user" + JsonSerializer.Serialize(user));
        var base64Image = await StreamToBase64Async(image, cancellationToken);
        var result = new VinaraBaseResponse<VinaraRetrieveUserInformationResponse>
        {
            Success = true,
            Timestamp = DateTime.UtcNow.ToString("O"),
            Result = new VinaraRetrieveUserInformationResponse
            {
                NationalCode = user.NationalCode!,
                Base64Image = base64Image,
                Gender = umKycLog.Gender.ToString(),
                PhoneNumber = user.MobileNumber,
            },
        };
        logger.LogInformation("***** My good result: " + JsonSerializer.Serialize(result));
        return result;
    }

    private VinaraBaseResponse<VinaraRetrieveUserInformationResponse> MakeErrorResponse(int code = (int)EResponseStatusCode.ItemIsNotFound, string message = "آیتم مورد نظر یافت نشد")
    {
        return new VinaraBaseResponse<VinaraRetrieveUserInformationResponse>
        {
            Timestamp = DateTime.UtcNow.ToString("O"),
            Success = false,
            Error = new VinaraError
            {
                Code = code,
                Message = message
            }
        };
    }
}