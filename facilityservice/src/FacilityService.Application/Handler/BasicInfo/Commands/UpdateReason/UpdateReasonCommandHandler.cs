using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Commands.UpdateReason;

public sealed class UpdateReasonCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time), IRequestHandler<UpdateReasonCommand, FacilityBaseMessageResponse<ReasonDto>>
{
    public async Task<FacilityBaseMessageResponse<ReasonDto>> Handle(UpdateReasonCommand request, CancellationToken ct)
    {
        var reason = await Db.Reasons.FirstOrDefaultAsync(r => r.Id == request.Id, ct) ?? throw FacilityDomainException.NotFound("مورد یافت نشد.");
        var before = reason.Title;
        reason.Update(request.Title, request.IsActive, request.SortOrder);
        Log(ChangeLogCategory.Company, "ویرایش فهرست اطلاعات پایه", reason.Title, null, new { title = before }, new { title = reason.Title, reason.IsActive });
        await Db.SaveChangesAsync(ct);
        return Ok(reason.ToDto());
    }
}
