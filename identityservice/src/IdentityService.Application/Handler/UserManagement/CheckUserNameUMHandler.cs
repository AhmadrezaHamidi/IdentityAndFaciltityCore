
//using IdentityService.Domain.Entity;

//namespace IdentityService.Application.Handler.UserManagement;

//public class CheckUserNameUMHandler : IBaseHandler<CheckUserNameUMRequest, BaseMessageResponse<CheckUserNameUMResponse>>
//{
//    private readonly IUserManagementRepository _userManagementRepository;
//    public CheckUserNameUMHandler(IUserManagementRepository userManagementRepository) =>  _userManagementRepository = userManagementRepository;

//    public async Task<BaseMessageResponse<CheckUserNameUMResponse>> Handle(CheckUserNameUMRequest request, CancellationToken cancellationToken)
//    {
//        var isExist = await _userManagementRepository.CheckExistByUserNameAsync(request.UserName, cancellationToken);
//        if (isExist) return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DuplicateUserNameForRegisterUM, new CheckUserNameUMResponse() { IsUmUser = isExist });

//        return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, new CheckUserNameUMResponse() { IsUmUser = isExist});
//    }
//}

//public record CheckUserNameUMRequest([Required] string UserName) : BaseCommand<BaseMessageResponse<CheckUserNameUMResponse>>;

//public class CheckUserNameUMResponse
//{
//    public bool IsUmUser { get; set; }
//}