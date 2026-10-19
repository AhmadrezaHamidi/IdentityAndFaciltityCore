using Microsoft.AspNetCore.Http;

namespace IdentityService.Services.ViewModels.Parameters
{
    public class LocalFileParameterViewModel : BaseViewModel
    {
        public IFormFile File { get; set; }
        public string FileString { get; set; }
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }
        public string BaseFileNameWithExtension { get; set; }
        public string Extension { get; set; }
        public string DirectoryPath { get; set; }
        public string RemoteDirectoryPath { get; set; }

    }
}
