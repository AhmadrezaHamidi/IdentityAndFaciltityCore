using IdentityService.Application.Handler.UserManagement;
using IdentityService.Application.Services.Kyc.Vinara;
using IdentityService.Domain.Models.Encryption;
using OmidBank.Common.Attribute.Swagger;

namespace IdentityService.Api.Controllers.V2.UserManagement;

[ApiVersion("2")]
public class UserManagementLogController(VinaraWageService vinaraWageService) :BaseIdentityApplicationApi
{
    [HttpPost]
    [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<GetLoginUMLogRequest, BaseMessageResponse<GetLoginUMLogResponse>>))]
    public async Task<IActionResult> LoginUMLog([FromBody] EncryptionDataModel request, CancellationToken cancellationToken)
    {
        CryptoSession<GetLoginUMLogRequest> decryptedRequest = _encryptionManager.Decrypt<GetLoginUMLogRequest>(request.EncryptedData, request.EncryptedKey, _baseRequest.RequestInfo);
        if (decryptedRequest == null)
            return BaseResult(_encryptionManager.Encrypt(JsonSerializer.Serialize(MessageResponse.GenerateMessageResponse(EResponseStatusCode.EncryptionError), JsonHelper.JsonCamelCaseOption()), request.EncryptedKey, _baseRequest.RequestInfo, request.EncryptedData));

        decryptedRequest.Value.CurrentUser = _baseRequest.CurrentUser;
        decryptedRequest.Value.RequestInfo = _baseRequest.RequestInfo;
        var response = await _mediator.Send(decryptedRequest.Value, cancellationToken);
        return BaseResultEncryption(response, decryptedRequest.Key, request.EncryptedData);
    }

    [HttpGet]
    public Task<IActionResult> GetKycWageAmount()
    {
        var wage = vinaraWageService.RetrieveWage();
        var data = new { KycWage = wage };
        return Task.FromResult(BaseResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, data)));
    }
}