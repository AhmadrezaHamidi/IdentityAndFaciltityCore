using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Commands.DeleteReason;

public sealed class DeleteReasonCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time), IRequestHandler<DeleteReasonCommand, FacilityBaseMessageResponse<bool>>
{
    public async Task<FacilityBaseMessageResponse<bool>> Handle(DeleteReasonCommand request, CancellationToken ct)
    {
        var reason = await Db.Reasons.FirstOrDefaultAsync(r => r.Id == request.Id && r.SchemeId == request.SchemeId, ct)
                     ?? throw FacilityDomainException.NotFound("مورد یافت نشد.");
        reason.Deactivate();
        Log(ChangeLogCategory.Company, "حذف از فهرست اطلاعات پایه", reason.Title, null);
        await Db.SaveChangesAsync(ct);
        return Ok(true);
    }
}
