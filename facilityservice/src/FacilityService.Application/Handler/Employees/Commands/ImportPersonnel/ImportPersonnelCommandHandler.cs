using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Employees.Commands.ImportPersonnel;

public sealed class ImportPersonnelCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : PersonnelHandlerBase(db, user, time), IRequestHandler<ImportPersonnelCommand, FacilityBaseMessageResponse<PersonnelImportResultDto>>
{
    public async Task<FacilityBaseMessageResponse<PersonnelImportResultDto>> Handle(ImportPersonnelCommand request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.PersonnelUpload);
        EnsureActive(company);
        var fileError = FileRules.Validate(request.File, FileRules.PersonnelImportExtensions, FileRules.TenMb, required: true, minSize: 1);
        if (fileError != null) throw Invalid(fileError);
        var rows = PersonnelSheetReader.Read(request.File!);
        if (rows.Count == 0) throw Invalid("فایل هیچ ردیف داده‌ای ندارد.");
        if (rows.Count > 5000) throw Invalid("حداکثر ۵۰۰۰ ردیف در هر فایل.");

        var existing = (await Db.Personnel.Where(p => p.CompanyId == company.Id && p.Status != PersonnelStatus.Deleted)
            .Select(p => p.NationalCode).ToListAsync(ct)).ToHashSet();
        var batch = new PersonnelImportBatch(company.Id, request.File!.FileName, Me, Now);
        Db.PersonnelImports.Add(batch);
        var errors = new List<PersonnelImportErrorDto>();
        var accepted = new List<Personnel>();
        foreach (var (row, cells) in rows)
        {
            string Cell(int i) => i < cells.Length ? cells[i] : "";
            var hire = PersonnelSheetReader.ParseDate(Cell(4));
            if (hire == null) { errors.Add(new(row, PersonnelSheetReader.Header[4], "تاریخ استخدام نامعتبر است (مثال: 1403/02/15).")); continue; }
            var priorityText = PersonnelSheetReader.Latin(Cell(5));
            var priority = string.IsNullOrEmpty(priorityText) ? 1 : int.TryParse(priorityText, out var pr) ? pr : -1;
            try
            {
                var data = new PersonnelData(Cell(0), PersonnelSheetReader.Latin(Cell(1)), PersonnelSheetReader.Latin(Cell(2)),
                    PersonnelSheetReader.Latin(Cell(3)), hire.Value, priority).Normalize(Today);
                if (!existing.Add(data.NationalCode)) { errors.Add(new(row, PersonnelSheetReader.Header[1], "این کد ملی قبلاً در این شرکت ثبت شده است.")); continue; }
                accepted.Add(Personnel.Create(company.Id, data, Me, Now));
            }
            catch (FacilityDomainException ex)
            {
                errors.Add(new(row, "", ex.Message));
            }
        }
        foreach (var p in accepted) Db.Personnel.Add(p);
        batch.Complete(rows.Count, accepted.Count, JsonSerializer.Serialize(errors, Json));
        Log(ChangeLogCategory.Personnel, "بارگذاری گروهی پرسنل",
            $"{company.Name}: {PersianText.Number(accepted.Count)} نفر از {PersianText.Number(rows.Count)} ردیف", company);
        await Db.SaveChangesAsync(ct);
        return Ok(new PersonnelImportResultDto(batch.Id, rows.Count, accepted.Count, errors.Count, errors));
    }
}
