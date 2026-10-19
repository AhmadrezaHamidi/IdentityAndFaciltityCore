using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Commands.CreateReason;

public sealed class CreateReasonCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time), IRequestHandler<CreateReasonCommand, FacilityBaseMessageResponse<ReasonDto>>
{
    public async Task<FacilityBaseMessageResponse<ReasonDto>> Handle(CreateReasonCommand request, CancellationToken ct)
    {
        if (!Enum.IsDefined(request.Type)) throw Invalid("نوع فهرست نامعتبر است.");
        var scheme = await Db.Schemes.FirstOrDefaultAsync(s => s.Id == request.SchemeId, ct)
                     ?? throw FacilityDomainException.NotFound("طرح یافت نشد.");
        var title = request.Title.Trim();
        if (await Db.Reasons.AnyAsync(r => r.SchemeId == scheme.Id && r.Type == request.Type && r.IsActive && r.Title == title, ct))
            throw Invalid("این عنوان در فهرست همین طرح وجود دارد.");
        var order = await Db.Reasons.Where(r => r.SchemeId == scheme.Id && r.Type == request.Type)
            .Select(r => (int?)r.SortOrder).MaxAsync(ct) ?? -1;
        var reason = new Reason(scheme.Id, request.Type, title, order + 1);
        Db.Reasons.Add(reason);
        Log(ChangeLogCategory.Company, "افزودن به فهرست اطلاعات پایه", $"{scheme.Title}: {title}", null);
        await Db.SaveChangesAsync(ct);
        return Ok(reason.ToDto());
    }
}
