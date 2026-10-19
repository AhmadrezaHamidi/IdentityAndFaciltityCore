using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Commands.DeleteScheme;

public sealed class DeleteSchemeCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time), IRequestHandler<DeleteSchemeCommand, FacilityBaseMessageResponse<bool>>
{
    public async Task<FacilityBaseMessageResponse<bool>> Handle(DeleteSchemeCommand request, CancellationToken ct)
    {
        var scheme = await Db.Schemes.FirstOrDefaultAsync(s => s.Id == request.SchemeId, ct)
                     ?? throw FacilityDomainException.NotFound("طرح یافت نشد.");
        if (!scheme.IsActive) return Ok(true, "این طرح از قبل غیرفعال است.");

        if (await Db.Schemes.CountAsync(s => s.IsActive, ct) <= 1)
            throw Invalid("آخرین طرح فعال حذف نمی‌شود؛ باید حداقل یک طرح فعال بماند.");

        var companies = await Db.Companies.CountAsync(c => c.SchemeId == scheme.Id && c.DeletedAt == null, ct);
        if (companies > 0)
            throw Invalid($"این طرح {PersianText.Number(companies)} شرکت فعال دارد و حذف نمی‌شود؛ ابتدا قرارداد آن‌ها را لغو کنید.");

        scheme.Deactivate();
        Log(ChangeLogCategory.Company, "حذف طرح", $"طرح {scheme.Title}", null, new { isActive = true }, new { isActive = false });
        await Db.SaveChangesAsync(ct);
        return Ok(true, "طرح حذف شد.");
    }
}
