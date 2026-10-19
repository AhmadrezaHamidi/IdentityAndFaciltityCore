using IdentityService.Application.Handler.Identities.Commands.ChangePassword;
using IdentityService.Application.Handler.Identities.Commands.ChangeUsername;
using IdentityService.Application.Handler.Identities.Commands.SendPasswordChangeOtp;
using IdentityService.Application.Handler.Identities.Commands.SendUsernameChangeOtp;
using IdentityService.Application.Handler.Identities.Commands.VerifyPasswordChangeOtp;
using IdentityService.Application.Handler.Identities.Commands.VerifyUsernameChangeOtp;
using IdentityService.Application.Handler.Identities.Queries.RetrieveSettingConfigurations;
using IdentityService.Application.Handler.UserManagement;
using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Contracts.Identities.ReadModels;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Models.Encryption;
using OmidBank.Common.Attribute.Swagger;
using OmidBank.Common.Base.MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityService.Api.Controllers.UserManagementKycLog
{
    [ApiVersion("2")]
    public class AuthenticationController : BaseIdentityApplicationApi
    {
        [HttpPost]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<RegisterUMRequest, BaseMessageResponse<RegisterUMResponse>>))]
        public async Task<IActionResult> RegisterUM([FromBody] EncryptionDataModel request, CancellationToken cancellationToken)
        {
            CryptoSession<RegisterUMRequest> decryptedRequest = _encryptionManager.Decrypt<RegisterUMRequest>(request.EncryptedData, request.EncryptedKey, _baseRequest.RequestInfo);
            if (decryptedRequest == null)
                return BaseResult(_encryptionManager.Encrypt(JsonSerializer.Serialize(MessageResponse.GenerateMessageResponse(EResponseStatusCode.EncryptionError), JsonHelper.JsonCamelCaseOption()), request.EncryptedKey, _baseRequest.RequestInfo, request.EncryptedData));

            decryptedRequest.Value.CurrentUser = _baseRequest.CurrentUser;
            decryptedRequest.Value.RequestInfo = _baseRequest.RequestInfo;
            var response = await _mediator.Send(decryptedRequest.Value, cancellationToken);

            return BaseResultEncryption(response, decryptedRequest.Key, request.EncryptedData);
        }

        [HttpGet]
        public async Task<IActionResult> CheckStatusUM([FromQuery] CheckStatusUmRequest query, CancellationToken cancellationToken)
        {
            return BaseResult(await _mediator.Send(query, cancellationToken));
        }

        [HttpGet]
        [Anonymous]
        public async Task<IActionResult> UpdateUM([FromQuery] UpdateUMRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (response.Data is not null)
            {
                var UMStatus = $"UMStatus={response.Data.UMStatus}";

                if (response.Data.AppId == Enums.AppEnum.PWA)
                    return Redirect($"{_configuration["UMPWACallBackUrl"]}?{UMStatus}");

                else if (response.Data.AppId == Enums.AppEnum.OWP)
                    return Redirect($"{_configuration["UMOWPCallBackUrl"]}&{UMStatus}");


                else if (response.Data.KycType == KycType.Certificate)
                    return Redirect($"{_configuration["CertificateMobileCallBackUrl"]}");

                else if (response.Data.AppId == Enums.AppEnum.MobileApp)
                    return Redirect($"{_configuration["UMMobileCallBackUrl"]}");
            }


            return BaseResult(response);
        }

        [HttpPost]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<LoginUMRequest, BaseMessageResponse<LoginUMHResponse>>))]
        public async Task<IActionResult> LoginUM([FromBody] EncryptionDataModel request, CancellationToken cancellationToken)
        {
            CryptoSession<LoginUMRequest> decryptedRequest =
                _encryptionManager.Decrypt<LoginUMRequest>(request.EncryptedData, request.EncryptedKey,
                    _baseRequest.RequestInfo);

            if (decryptedRequest == null)
                return BaseResult(_encryptionManager.Encrypt(JsonSerializer.Serialize(MessageResponse.GenerateMessageResponse(EResponseStatusCode.EncryptionError), JsonHelper.JsonCamelCaseOption()), request.EncryptedKey,
                    _baseRequest.RequestInfo, request.EncryptedData));

            var requestData = new LoginUMRequest(decryptedRequest.Value.UserName, decryptedRequest.Value.Password,
                request.EncryptedData);
            requestData.CurrentUser = _baseRequest.CurrentUser;
            requestData.RequestInfo = _baseRequest.RequestInfo;
            var response = await _mediator.Send(requestData, cancellationToken);

            return BaseResultEncryption(response, decryptedRequest.Key, request.EncryptedData);
        }

        [HttpPost]
        [UMAuthorization]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<LogoutUMRequest, BaseMessageResponse<LogoutUMResponse>>))]
        public async Task<IActionResult> LogoutUM([FromBody] EncryptionDataModel request, CancellationToken cancellationToken)
        {
            CryptoSession<LogoutUMRequest> decryptedRequest =
                _encryptionManager.Decrypt<LogoutUMRequest>(request.EncryptedData, request.EncryptedKey,
                    _baseRequest.RequestInfo);

            if (decryptedRequest == null)
                return BaseResult(_encryptionManager.Encrypt(
                    JsonSerializer.Serialize(
                        MessageResponse.GenerateMessageResponse(EResponseStatusCode.EncryptionError),
                        JsonHelper.JsonCamelCaseOption()), request.EncryptedKey,
                    _baseRequest.RequestInfo, request.EncryptedData));

            decryptedRequest.Value.CurrentUser = _baseRequest.CurrentUser;
            decryptedRequest.Value.RequestInfo = _baseRequest.RequestInfo;
            var response = await _mediator.Send(decryptedRequest.Value, cancellationToken);

            return BaseResultEncryption(response, decryptedRequest.Key, request.EncryptedData);
        }


        [HttpGet]
        public async Task<IActionResult> CheckStatusForgetPassword([FromQuery] CheckStatusForgetPasswordRequest query, CancellationToken cancellationToken)
        {
            return BaseResult(await _mediator.Send(query, cancellationToken));
        }


        [HttpPost]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<TemporaryLoginRequest, BaseMessageResponse<TemporaryLoginResponse>>))]
        public async Task<IActionResult> TemporaryLogin([FromBody] EncryptionDataModel request, CancellationToken cancellationToken)

        {
            CryptoSession<TemporaryLoginRequest> decryptedRequest = _encryptionManager.Decrypt<TemporaryLoginRequest>(request.EncryptedData, request.EncryptedKey, _baseRequest.RequestInfo);
            if (decryptedRequest == null)
                return BaseResult(_encryptionManager.Encrypt(JsonSerializer.Serialize(MessageResponse.GenerateMessageResponse(EResponseStatusCode.EncryptionError), JsonHelper.JsonCamelCaseOption()), request.EncryptedKey, _baseRequest.RequestInfo, request.EncryptedData));

            decryptedRequest.Value.CurrentUser = _baseRequest.CurrentUser;
            decryptedRequest.Value.RequestInfo = _baseRequest.RequestInfo;
            var response = await _mediator.Send(decryptedRequest.Value, cancellationToken);

            return BaseResultEncryption(response, decryptedRequest.Key, request.EncryptedData);
        }

        [HttpPost]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<RegisterByCardRequest, BaseMessageResponse<RegisterByCardResponse>>))]
        public async Task<IActionResult> RegisterByCard([FromBody] EncryptionDataModel request, CancellationToken cancellationToken)

        {
            CryptoSession<RegisterByCardRequest> decryptedRequest =
                _encryptionManager.Decrypt<RegisterByCardRequest>(request.EncryptedData, request.EncryptedKey,
                    _baseRequest.RequestInfo);
            if (decryptedRequest == null)
                return BaseResult(_encryptionManager.Encrypt(
                    JsonSerializer.Serialize(
                        MessageResponse.GenerateMessageResponse(EResponseStatusCode.EncryptionError),
                        JsonHelper.JsonCamelCaseOption()), request.EncryptedKey, _baseRequest.RequestInfo,
                    request.EncryptedData));

            decryptedRequest.Value.CurrentUser = _baseRequest.CurrentUser;
            decryptedRequest.Value.RequestInfo = _baseRequest.RequestInfo;
            var response = await _mediator.Send(decryptedRequest.Value, cancellationToken);

            return BaseResultEncryption(response, decryptedRequest.Key, request.EncryptedData);
        }

        [HttpPost]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<TwoPhaseloginDataRequest, BaseMessageResponse<TwoPhaseloginResponse>>))]
        public async Task<IActionResult> TwoPhaselogin([FromBody] EncryptionDataModel request, CancellationToken cancellationToken)
        {
            CryptoSession<TwoPhaseloginDataRequest> decryptedRequest =
                _encryptionManager.Decrypt<TwoPhaseloginDataRequest>(request.EncryptedData, request.EncryptedKey,
                    _baseRequest.RequestInfo);

            if (decryptedRequest == null)
                return BaseResult(_encryptionManager.Encrypt(JsonSerializer.Serialize(MessageResponse.GenerateMessageResponse(EResponseStatusCode.EncryptionError), JsonHelper.JsonCamelCaseOption()), request.EncryptedKey,
                    _baseRequest.RequestInfo, request.EncryptedData));

            var requestData = new TwoPhaseloginDataRequest(decryptedRequest.Value.OtpCode,
                request.EncryptedData);
            requestData.CurrentUser = _baseRequest.CurrentUser;
            requestData.RequestInfo = _baseRequest.RequestInfo;
            var response = await _mediator.Send(requestData, cancellationToken);

            return BaseResultEncryption(response, decryptedRequest.Key, request.EncryptedData);
        }

        [HttpPost]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<RegisterUMUserRequest, BaseMessageResponse<RegisterUMUserResponse>>))]
        public async Task<IActionResult> RegisterUMUser([FromBody] EncryptionDataModel request, CancellationToken cancellationToken)
        {
            CryptoSession<RegisterUMUserRequest> decryptedRequest = _encryptionManager.Decrypt<RegisterUMUserRequest>(request.EncryptedData, request.EncryptedKey, _baseRequest.RequestInfo);
            if (decryptedRequest == null)
                return BaseResult(_encryptionManager.Encrypt(JsonSerializer.Serialize(MessageResponse.GenerateMessageResponse(EResponseStatusCode.EncryptionError), JsonHelper.JsonCamelCaseOption()), request.EncryptedKey, _baseRequest.RequestInfo, request.EncryptedData));

            decryptedRequest.Value.CurrentUser = _baseRequest.CurrentUser;
            decryptedRequest.Value.RequestInfo = _baseRequest.RequestInfo;
            var response = await _mediator.Send(decryptedRequest.Value, cancellationToken);

            return BaseResultEncryption(response, decryptedRequest.Key, request.EncryptedData);
        }

        [HttpPost]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<SendOtpUMRequest, BaseMessageResponse<SendOtpUMResponse>>))]
        public async Task<IActionResult> SendOtpUM([FromBody] EncryptionDataModel request, CancellationToken cancellationToken)
        {
            CryptoSession<SendOtpUMRequest> decryptedRequest = _encryptionManager.Decrypt<SendOtpUMRequest>(request.EncryptedData, request.EncryptedKey, _baseRequest.RequestInfo);
            if (decryptedRequest == null)
                return BaseResult(_encryptionManager.Encrypt(JsonSerializer.Serialize(MessageResponse.GenerateMessageResponse(EResponseStatusCode.EncryptionError), JsonHelper.JsonCamelCaseOption()), request.EncryptedKey, _baseRequest.RequestInfo, request.EncryptedData));

            decryptedRequest.Value.CurrentUser = _baseRequest.CurrentUser;
            decryptedRequest.Value.RequestInfo = _baseRequest.RequestInfo;
            var response = await _mediator.Send(decryptedRequest.Value, cancellationToken);

            return BaseResultEncryption(response, decryptedRequest.Key, request.EncryptedData);
        }
        
        [HttpPost("/v{version:apiVersion}/[controller]/users/username-change/otp")]
        [UMAuthorization]
        [SwaggerOperation(Summary = "وب سرویس ارسال رمز یکبار مصرف برای تغییر نام کاربری")]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<SendUsernameChangeOtpCommand, BaseMessageResponse<SendUsernameChangeOtpDto>>))]
        public async Task<IActionResult> SendUsernameChangeOtp([FromBody]EncryptionDataModel request, CancellationToken cancellationToken)
        {
            return await HandleEncryptedRequests<SendUsernameChangeOtpCommand, SendUsernameChangeOtpDto>(request, cancellationToken); 
        }
        
        [HttpPost("/v{version:apiVersion}/[controller]/users/username-change/otp/verify")]
        [UMAuthorization]
        [SwaggerOperation(Summary = "وب سرویس صحت سنجی رمز یکبار مصرف برای تغییر نام کاربری")]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<VerifyUsernameChangeOtpCommand, BaseMessageResponse<VerifyUsernameChangeOtpDto>>))]
        public async Task<IActionResult> VerifyUsernameChangeOtp([FromBody]EncryptionDataModel request, CancellationToken cancellationToken)
        {
            return await HandleEncryptedRequests<VerifyUsernameChangeOtpCommand, VerifyUsernameChangeOtpDto>(request, cancellationToken);
        }
        
        [HttpPut("/v{version:apiVersion}/[controller]/users/username")]
        [UMAuthorization]
        [SwaggerOperation(Summary = "وب سرویس تغییر نام کاربری")]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<ChangeUsernameCommand, BaseMessageResponse<ChangeUsernameDto>>))]
        public async Task<IActionResult> ChangeUsername([FromBody]EncryptionDataModel request, CancellationToken cancellationToken)
        {
            return await HandleEncryptedRequests<ChangeUsernameCommand, ChangeUsernameDto>(request, cancellationToken);
        }
        
        [HttpPost("/v{version:apiVersion}/[controller]/users/password-change/otp")]
        [UMAuthorization]
        [SwaggerOperation(Summary = "وب سرویس ارسال رمز یکبار مصرف برای تغییر رمز ورود")]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<SendPasswordChangeOtpCommand, BaseMessageResponse<SendPasswordChangeOtpDto>>))]
        public async Task<IActionResult> SendPasswordChangeOtp([FromBody]EncryptionDataModel request, CancellationToken cancellationToken)
        {
            return await HandleEncryptedRequests<SendPasswordChangeOtpCommand, SendPasswordChangeOtpDto>(request, cancellationToken);
        }
        
        [HttpPost("/v{version:apiVersion}/[controller]/users/password-change/otp/verify")]
        [UMAuthorization]
        [SwaggerOperation(Summary = "وب سرویس صحت سنجی رمز یکبار مصرف برای تغییر رمز ورود")]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<VerifyPasswordChangeOtpCommand, BaseMessageResponse<VerifyPasswordChangeOtpDto>>))]
        public async Task<IActionResult> VerifyPasswordChangeOtp([FromBody]EncryptionDataModel request, CancellationToken cancellationToken)
        {
            return await HandleEncryptedRequests<VerifyPasswordChangeOtpCommand, VerifyPasswordChangeOtpDto>(request, cancellationToken);
        }
        
        [HttpPut("/v{version:apiVersion}/[controller]/users/password")]
        [UMAuthorization]
        [SwaggerOperation(Summary = "وب سرویس تغییر رمز")]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<ChangePasswordCommand, BaseMessageResponse<ChangePasswordDto>>))]
        public async Task<IActionResult> ChangePassword([FromBody]EncryptionDataModel request, CancellationToken cancellationToken)
        {
            return await HandleEncryptedRequests<ChangePasswordCommand, ChangePasswordDto>(request, cancellationToken);
        }
        
        [HttpPost("/v{version:apiVersion}/[controller]/users/settings")]
        [UMAuthorization]
        [SwaggerOperation(Summary = "وب سرویس دریافت مقادیر تنظیمات کاربر")]
        [BaseSwaggerOperationFilter(typeof(AutoGenerateEncryptDocument<RetrieveSettingConfigurationsQuery, BaseMessageResponse<RetrieveSettingConfigurationsDto>>))]
        public async Task<IActionResult> RetrieveSettingConfigurations([FromBody]EncryptionDataModel request, CancellationToken cancellationToken)
        {
            return await HandleEncryptedRequests<RetrieveSettingConfigurationsQuery, RetrieveSettingConfigurationsDto>(request, cancellationToken);
        }
            
        private async Task<IActionResult> HandleEncryptedRequests<TRequest, TResponse>(EncryptionDataModel encryptedRequest, CancellationToken cancellationToken) where TRequest : BaseCommand<BaseMessageResponse<TResponse>>
        {
            var decryptedRequest = _encryptionManager.Decrypt<TRequest>(encryptedRequest.EncryptedData, encryptedRequest.EncryptedKey, _baseRequest.RequestInfo);
            if (decryptedRequest == null)
                return EncryptionErrorResponse(encryptedRequest);
            var request = decryptedRequest.Value;
            request.CurrentUser = _baseRequest.CurrentUser;
            request.RequestInfo = _baseRequest.RequestInfo;
            var response = await _mediator.Send(request, cancellationToken);
            return BaseResultEncryption(response, decryptedRequest.Key, encryptedRequest.EncryptedData);
        }
        
        private IActionResult EncryptionErrorResponse(EncryptionDataModel request)
        {
            return BaseResult(_encryptionManager.Encrypt(JsonSerializer.Serialize(MessageResponse.GenerateMessageResponse(EResponseStatusCode.EncryptionError), JsonHelper.JsonCamelCaseOption()), request.EncryptedKey,
                _baseRequest.RequestInfo, request.EncryptedData));
        }
        
        private async Task<IActionResult> HandlePlainRequests<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken) where TRequest : BaseCommand<BaseMessageResponse<TResponse>>
        {
            request.CurrentUser = _baseRequest.CurrentUser;
            request.RequestInfo = _baseRequest.RequestInfo;
            var response = await _mediator.Send(request, cancellationToken);
            return BaseResult(response);
        }
    }
}