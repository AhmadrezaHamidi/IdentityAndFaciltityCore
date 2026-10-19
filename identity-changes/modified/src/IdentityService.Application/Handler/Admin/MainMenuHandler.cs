using IdentityService.Domain.IRepositories;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.Cryptography;
using OmidBank.Common.JwtHelper;

namespace IdentityService.Application.Handler.Admin
{
    public class MainMenuHandler : IBaseHandler<MainMenuQuery,
        BaseMessageResponse<List<IdentityService.Domain.Models.User.PanelMenuModel?>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly string _jwtTokenEncryptorKey;
        private readonly IJwtService _jwtService;
        private readonly IPanelMenuRepository _panelMenuRepository;

        public MainMenuHandler(IConfiguration configuration, IUserRepository userRepository, IJwtService jwtService
            , IPanelMenuRepository panelMenuRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _jwtTokenEncryptorKey = configuration["JwtTokenEncryptorKey"];
            _panelMenuRepository = panelMenuRepository;
        }

        public async Task<BaseMessageResponse<List<IdentityService.Domain.Models.User.PanelMenuModel>>>
            Handle(MainMenuQuery query, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<List<IdentityService.Domain.Models.User.PanelMenuModel>>(EResponseStatusCode.DoneSuccessfully, new());
            var authRequest = new TokenViewModel
            {
                Token = query.token,
            };
            var checkAuthenticate = await PanelAuthenticateForToken(authRequest, cancellationToken);
            if (!checkAuthenticate.Item1)
                return null;

            var menus = await _panelMenuRepository.GetPanelRoleMenuAsyncv2(checkAuthenticate.Item4, cancellationToken);

            // PanelMenu هم تب است هم مجوز: ردیف‌های عملیات (تأیید، رد، حذف — Kind=2) آدرس ندارند
            // و فقط برای اعطای دسترسی وجود دارند. اگر فیلتر نشوند، در sidebar به‌عنوان آیتم منو
            // ظاهر می‌شوند. معیار پذیرش ۱ سند: «بخش‌های بدون دسترسی کاملاً مخفی باشند».
            response.Data = menus.Where(m => !string.IsNullOrWhiteSpace(m.Url)).ToList();
            return response;
        }

        private async Task<Tuple<bool, string, long, string, int>> PanelAuthenticateForToken(TokenViewModel token, CancellationToken cancellationToken)
        {
            var valid = _jwtService.ValidateToken(token.Token);
            var queryResult = new Tuple<bool, string, long, string, int>(false, "دسترسی این قسمت از سرویس برای رول کاربر تعریف نشده است", 0, "", valid.Item4); //valid-Message-UserId-UserName-valid Code for panel
            if (!valid.Item1)
            {
                queryResult = new Tuple<bool, string, long, string, int>(valid.Item1, valid.Item2, 0, "", valid.Item4);
                return queryResult;
            }

            queryResult = new Tuple<bool, string, long, string, int>(true, "مجوز صادر شد", valid.Item3.UserId, valid.Item3.UserName, 702);
            return queryResult;

        }
    }

    public sealed record MainMenuQuery(string token) : BaseQuery<BaseMessageResponse<List<IdentityService.Domain.Models.User.PanelMenuModel>>>;

    public sealed class PanelMenuModel
    {
        public int Id { get; private set; }
        public int? ParentId { get; private set; }
        public string Title { get; private set; }
        public string EnTitle { get; private set; }
        public string Url { get; private set; }
        public string IconName { get; private set; }
        public string ActionName { get; private set; }
        public string ControllerName { get; private set; }
        public bool IsRootMenu { get; private set; }
        public bool IsInternalMenu { get; private set; }
        public DateTime CreationTime { get; private set; }
        public bool IsActive { get; private set; }
    }
}