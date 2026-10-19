using Microsoft.AspNetCore.Http;

namespace IdentityService.Services.ViewModels.InternalAuth
{
    public class UploadVideoViewModel : BaseViewModel
    {
        public IFormFile File { get; set; }
        public bool? AddEntityMode { get; set; }
        public int? EntityId { get; set; }
        public string Title { get; set; }
        public string FileString { get; set; }
        public string FileInfo { get; set; }
        public string MobileNumber { get; set; }
    }
}
