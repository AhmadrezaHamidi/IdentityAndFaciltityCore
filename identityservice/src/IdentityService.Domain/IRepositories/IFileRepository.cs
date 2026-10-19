using System.Threading;

namespace IdentityService.Domain.IRepositories
{
    public interface IFileRepository : IBaseRepository
    {
        FileModel GetFileItemById(int id, CancellationToken cancellationToken);
        FileModel GetFileItemByMobileNumber(string mobileNumber, CancellationToken cancellationToken);
        List<FileModel> GetAllFileItem(CancellationToken cancellationToken);
        int InsertFileItem(FileModel fileItem, CancellationToken cancellationToken);
        int DeleteFileItem(int id, CancellationToken cancellationToken);
    }
}
