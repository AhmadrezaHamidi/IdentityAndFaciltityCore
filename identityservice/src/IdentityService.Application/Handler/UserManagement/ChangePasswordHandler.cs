
//using System.Threading;
//using IdentityService.Domain.Entity;
//using OmidBank.Common.Extentions;
//using OmidBank.Common.InternalThreadParty.OmidTech.Request;
//using OmidBank.Common.InternalThreadParty.OmidTech;
//using IdentityService.Domain.Entity.Person;
//using Microsoft.Extensions.Configuration;

//namespace IdentityService.Application.Handler.UserManagement;

//public class ChangePasswordHandler : IBaseHandler<ChangePasswordUMRequest, BaseMessageResponse<ChangePasswordUMResponse>>
//{
//    private readonly IUserManagementRepository _userManagementRepository;
//    private readonly IPersonRepository _personRepository;
//    private readonly IConfiguration _configuration;
//    public ChangePasswordHandler(IUserManagementRepository userManagementRepository, IPersonRepository personRepository, IConfiguration configuration)
//    {
//        _userManagementRepository = userManagementRepository;
//        _personRepository = personRepository;
//        _configuration = configuration;
//    }

//    public async Task<BaseMessageResponse<ChangePasswordUMResponse>> Handle(ChangePasswordUMRequest request, CancellationToken cancellationToken)
//    {       
//        var response = MessageResponse.GenerateMessageResponse<ChangePasswordUMResponse>(EResponseStatusCode.DoneSuccessfully);

//        var userManagement = await _userManagementRepository.GetByUserIdAsync(request.CurrentUser.Id, cancellationToken);
//        if (userManagement is null)
//            return MessageResponse.GenerateMessageResponse<ChangePasswordUMResponse>(EResponseStatusCode.ItemIsNotFound);

//        if (userManagement.Password != CommonHelper.CreateMD5(request.CurrentPassword))
//            return MessageResponse.GenerateMessageResponse<ChangePasswordUMResponse>(EResponseStatusCode.WrongPassword); 

//        if (!ValidatePassword(request.NewPassword))
//            return MessageResponse.GenerateMessageResponse<ChangePasswordUMResponse>(EResponseStatusCode.NotValidPassword);

//        if (userManagement.Password =***; 

        
//        await HandleUpdatePassword(userManagement,request.NewPassword,cancellationToken);
//        await HandleSms(userManagement.UserName, request.CurrentUser.NationalCode, cancellationToken);

//        return response;
//    }

//    private async Task HandleUpdatePassword(UserManagmentEntity entity, string newPassword, CancellationToken cancellationToken)
//    {
//        entity.Password = ***;
//        await _userManagementRepository.UpdateAsync(entity,cancellationToken);
//        await _userManagementRepository.SaveChangeAsync(cancellationToken);
//    }
//    private bool ValidatePassword(string password)
//    {
//        if (password.IsNullOrEmpty()) return false;
//        return CommonHelper.IsValidPassword(password);
//    }

//    private async Task HandleSms(string userName,string nationalCode,CancellationToken cancellationToken)
//    {
//        var person = await _personRepository.GetByNationalCodeAsync(nationalCode, cancellationToken);
//        OmidTechProvider omidTechProvider = new OmidTechProvider(_configuration["BaseOmidTechUrl"], _configuration["ApiKeyOmidTech"]);
//        var messageRequest = new SendMessageRequest
//        {
//            MobileNumbers = new List<string>() { person.MobileNumber },
//            SmsType = "omidBoom",
//            Scheduler = null,
//            TerminalType = 18
//        };

//        var now = DateTime.Now;
//        var date = new PersianDateTime(now).ToString("yyyy/MM/dd");
//        var time = new PersianDateTime(now).ToString("HH:mm:ss");

//        messageRequest.MessageBody = $"امید بانک سپه\nتغییر رمز خدمات بانکی\nمشتری گرامی رمز ورود به خدمات بانکی شما با موفقیت تغییر یافت\nنام کاربری:\n{userName}\n{date}\n{time}"; 
//        //await omidTechProvider.SendSms(messageRequest, cancellationToken);
//    }
//}

//public record ChangePasswordUMRequest([Required] string CurrentPassword, [Required] string NewPassword) : BaseCommand<BaseMessageResponse<ChangePasswordUMResponse>>;

//public class ChangePasswordUMResponse { }




