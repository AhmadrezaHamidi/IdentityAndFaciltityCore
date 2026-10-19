using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.Person;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OmidBank.Common.Cache;
using OmidBank.Common.InternalThirdParty.SmsService;
using OmidBank.Common.InternalThirdParty.SmsService.Request;

namespace IdentityService.Application.Services.Kyc.Common;

public class KycSmsHelper(
    IPersonRepository personRepository,
    IOptions<SmsProviderOptions> smsProviderOptions,
    IRedisCacheManager redisCacheManager,
    HttpClient httpClient,
    IConfiguration configuration,
    ISmsThrottleService smsThrottleService)
{
    public async Task SendInquiryResultBySmsAsync(UserManagementKycLogEntity kycLog, CancellationToken cancellationToken)
    {
        var person = await personRepository.GetByNationalCodeAsync(kycLog.User!.Person.NationalCode, cancellationToken);
        var sendSmsRequest = new SendActivitionCodeOmidTechViewModel
        {
            MobileNumber = person.MobileNumber,
            Scheduler = null,
            Code = null,
            Url = configuration["MessageServiceUrl"]!,
            ApiKeyInternal = configuration["ApiKeyInternal"]!,
            Message = GetMessageText(kycLog.KycType!.Value, kycLog.KycStep)
        };

        if (string.IsNullOrEmpty(sendSmsRequest.Message))
            return;
        await SendMessage(sendSmsRequest, cancellationToken);
    }

    private static string GetMessageText(KycType kycType, KycStep kycStep)
    {
        var now = DateTime.Now;
        var date = new PersianDateTime(now).ToString("yyyy/MM/dd");
        var time = new PersianDateTime(now).ToString("HH:mm");
        if (kycStep is KycStep.NotExistUser or KycStep.GenerateLink or KycStep.Pending or KycStep.IsNotBankCustomer or KycStep.IsNotBankCustomerEnableCreateCustomer or KycStep.CustomerPending)
            return string.Empty;
        return kycType switch
        {
            KycType.RegisterUM => kycStep == KycStep.Rejected
                ? $"امید بانک سپه\nمشتری گرامی هویت شما جهت ورود به خدمات بانکی تایید نشد\n{date}\n{time}"
                : $"امید بانک سپه\n مشتری گرامی هویت شما تأیید شد.\nجهت تعیین نام کاربری و رمز ورود خدمات بانکی به اپلیکیشن امید بانک مراجعه نمایید.\n{date}\n{time}",
            KycType.ForgetPasswordUM => kycStep == KycStep.Rejected
                ? $"امید بانک سپه\nفراموشی رمز خدمات بانکی\nمشتری گرامی هویت شما تایید نشد\n{date}\n{time}"
                : $"امید بانک سپه\nورود به خدمات بانکی\nمشتری گرامی هویت شما تایید شد{date}\n{time}",
            _ => string.Empty
        };
    }

    private async Task SendMessage(SendActivitionCodeOmidTechViewModel viewModel, CancellationToken cancellationToken)
    {
        try
        {
            var smsServiceProvider = new SmsServiceProvider(configuration["UserNameAteye"], configuration["PasswordAteye"], configuration,
                smsProviderOptions, redisCacheManager, httpClient, smsThrottleService);
            var mobile = new List<string> { viewModel.MobileNumber };
            var requestSendSms = new OmidBank.Common.InternalThreadParty.OmidTech.Request.SendMessageRequest
            {
                MobileNumbers = mobile,
                MessageBody = viewModel.Message,
                SmsType = "omidBoom",
                Scheduler = viewModel.Scheduler,
                TerminalType = 18
            };
            await smsServiceProvider.SmsSender(requestSendSms, cancellationToken);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}