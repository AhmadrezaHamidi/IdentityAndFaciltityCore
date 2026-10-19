namespace IdentityService.Services.IServices
{
    public interface IFileService : IBaseService
    {
        Task<MemoryStream> DownloadLocalFileMemoryStream(string filePath, CancellationToken cancellationToken);
        Task<byte[]> DownloadLocalFileByteArray(string filePath, CancellationToken cancellationToken);
        Task<string> ByteToDisplaySize(long value, int decimalPlaces = 1, CancellationToken cancellationToken = default);
        Task<Enums.FileTypesEnum> GetFileType(string mimeType, CancellationToken cancellationToken);
        Task<string> GetFileExtension(string fileName, CancellationToken cancellationToken);
        Task<string> GetMimeType(string fileName, CancellationToken cancellationToken);
        Task<string> GetFileBaseUrl(string fileUsageType, CancellationToken cancellationToken);
        Task<string> GetFileFullUrl(string fileUsageType, string fileNameWithExtension, int? fileId, CancellationToken cancellationToken);
        void SaveFileSync(LocalFileParameterViewModel fileParameters, CancellationToken cancellationToken);
        Task<MemoryStream> DownloadFile(string filePath, CancellationToken cancellationToken);
        Task<bool> CheckInvitedCode(string InvitedCode);
    }
}
