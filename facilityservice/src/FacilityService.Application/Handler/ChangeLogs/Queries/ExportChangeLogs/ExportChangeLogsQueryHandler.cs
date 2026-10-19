using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.ChangeLogs.Queries.ExportChangeLogs;

public sealed class ExportChangeLogsQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : ChangeLogHandlerBase(db, user, time), IRequestHandler<ExportChangeLogsQuery, FacilityBaseMessageResponse<FileUpload>>
{
    public async Task<FacilityBaseMessageResponse<FileUpload>> Handle(ExportChangeLogsQuery request, CancellationToken ct)
    {
        var items = await Filter(request.Category, request.CompanyId, request.PanelUserId, request.From, request.To, request.Term)
            .OrderByDescending(l => l.Id).Take(50_000).ToListAsync(ct);
        return Ok(Csv.File("change-log.csv", ["زمان", "کاربر", "کد کاربر", "شعبه", "عملیات", "جزئیات", "مقدار قبلی", "مقدار جدید"],
            items.Select(l => new object?[] { l.At, l.User.UserName, l.User.UserCode, l.User.BranchName, l.Action, l.Details, l.Before, l.After })));
    }
}
