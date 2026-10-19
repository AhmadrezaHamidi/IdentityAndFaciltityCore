using XAct;

namespace IdentityService.Application.Services.Kyc.Common;

public class FileBasedStorage
{
    public async Task UploadAsync(byte[] bytes, string objectName, int fileType, CancellationToken cancellationToken = default)
    {
        await File.WriteAllBytesAsync($"./VinaraImage/{objectName}", bytes, cancellationToken);
    }

    private async Task UploadFile(Stream stream, string objectName, int fileType, CancellationToken cancellationToken = default)
    {
        await File.WriteAllBytesAsync($"./VinaraImage/{objectName}", stream.ReadAllBytes(), cancellationToken);
    }

    public async Task<Stream> DownloadAsync(string objectName)
    {
        var bytes = await File.ReadAllBytesAsync($"./VinaraImage/{objectName}");
        return new MemoryStream(bytes);
    }

    public async Task<string> GetDownloadUrlAsync(string objectName, int expiryInSeconds = 300)
    {
        return "./VinaraImage/" + objectName;
    }
}