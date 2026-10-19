using FacilityService.Application.Common;

namespace FacilityService.Application.Handler.Loans.Queries.DownloadLoanDocument;

/// <summary>
/// دانلود مستند یک درخواست وام. بدون این، فهرست‌ها شناسهٔ فایل را می‌دادند و راهی برای
/// گرفتنش نبود.
/// </summary>
public sealed record DownloadLoanDocumentQuery(long RequestId, long FileId) : FacilityBaseQuery<FacilityBaseMessageResponse<FileUpload>>;
