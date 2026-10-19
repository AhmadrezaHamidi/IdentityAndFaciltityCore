namespace IdentityService.Services.Services
{
    public class AvatarImageService : BaseService, IAvatarImageService
    {
        private readonly ILogger<PannelAuthService> _logger;
        private readonly IRequestHelper _requestHelper;

        public AvatarImageService(ILogger<PannelAuthService> logger, IRequestHelper requestHelper)
        {
            _logger = logger;
            _requestHelper = requestHelper;
        }

        public bool UpdateAvatarImage(string imageFile, string fileName, string oldFileName, string path,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(imageFile)) 
                return false;
            
            //var rndNumber = CommonHelper.GenerateLongRandomNumber();
            var fullFileName = $"{path}/{fileName}";
            var status = CommonHelper.SaveImage(imageFile, fullFileName, true, 0, 0, true);
            if (!status) 
                return false;
            
            if (!string.IsNullOrEmpty(oldFileName)) // Refactor: this line could be removed.
                DeleteAvatarFile(oldFileName, path, cancellationToken);
            return true;

        }

        public void DeleteAvatarFile(string fileName, string path, CancellationToken cancellationToken)
        {
            try
            {
                var fullFileName = $"{path}/{fileName}";
                if (System.IO.File.Exists(fullFileName))
                    System.IO.File.Delete(fullFileName);
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
            }
        }
    }
}