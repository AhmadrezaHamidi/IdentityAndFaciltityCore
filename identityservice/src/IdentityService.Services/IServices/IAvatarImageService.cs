namespace IdentityService.Services.IServices
{
    public interface IAvatarImageService : IBaseService
    {
        bool UpdateAvatarImage(string imageFile, string fileName, string oldFileName, string path, CancellationToken cancellationToken);
        void DeleteAvatarFile(string fileName, string path, CancellationToken cancellationToken);
    }
}
