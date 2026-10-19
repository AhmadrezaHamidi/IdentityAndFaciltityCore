using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Loans.Queries.DownloadLoanDocument;

public sealed class DownloadLoanDocumentQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time, IFacilityFileStorage storage)
    : FacilityPanelHandlerBase(db, user, time), IRequestHandler<DownloadLoanDocumentQuery, FacilityBaseMessageResponse<FileUpload>>
{
    public async Task<FacilityBaseMessageResponse<FileUpload>> Handle(DownloadLoanDocumentQuery request, CancellationToken ct)
    {
        // همان قاعدهٔ دیدن درخواست: خارج از دامنه «یافت نشد» می‌گیرد تا وجودش لو نرود.
        var loan = await Db.LoanRequests.Include(l => l.Documents).ThenInclude(d => d.File)
            .FirstOrDefaultAsync(l => l.Id == request.RequestId, ct);
        if (loan == null || !CanOn(K.LoansView, loan) || !CanSeeStage(loan))
            throw FacilityDomainException.NotFound("درخواست یافت نشد.");

        var doc = loan.Documents.FirstOrDefault(d => d.FileId == request.FileId)
                  ?? throw FacilityDomainException.NotFound("مستند یافت نشد.");
        return Ok(await ReadAsync(storage, doc.File, ct));
    }
}
