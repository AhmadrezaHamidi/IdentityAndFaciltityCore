using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.ChangeLogs.Queries.GetChangeLogUsers;

public sealed class GetChangeLogUsersQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : ChangeLogHandlerBase(db, user, time), IRequestHandler<GetChangeLogUsersQuery, FacilityBaseMessageResponse<IReadOnlyList<RegistrarDto>>>
{
    public async Task<FacilityBaseMessageResponse<IReadOnlyList<RegistrarDto>>> Handle(GetChangeLogUsersQuery request, CancellationToken ct)
    {
        // آخرین نام هر کاربر: Registrar عکس فوری لحظهٔ عملیات است، پس یک کاربر می‌تواند
        // در ردیف‌های قدیمی نام یا شعبهٔ دیگری داشته باشد. دو پرس‌وجوی ساده، چون
        // گروه‌بندی روی یک نوع Owned به SQL ترجمه نمی‌شود.
        var scoped = Scoped();
        var lastIds = await scoped.GroupBy(l => l.UserId).Select(g => g.Max(l => l.Id)).ToListAsync(ct);
        var rows = await Db.ChangeLogs.Where(l => lastIds.Contains(l.Id)).Select(l => l.User).ToListAsync(ct);

        return Ok<IReadOnlyList<RegistrarDto>>(rows.Select(r => r.ToDto())
            .OrderBy(r => r.UserName, StringComparer.Ordinal).ToList());
    }
}
