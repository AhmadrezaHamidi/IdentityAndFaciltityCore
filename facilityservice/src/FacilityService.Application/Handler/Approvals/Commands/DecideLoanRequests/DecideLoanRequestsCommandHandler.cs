using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Approvals.Commands.DecideLoanRequests;

public sealed class DecideLoanRequestsCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time, IFacilityFileStorage storage)
    : ApprovalHandlerBase(db, user, time), IRequestHandler<DecideLoanRequestsCommand, FacilityBaseMessageResponse<DecisionResultDto>>
{
    public async Task<FacilityBaseMessageResponse<DecisionResultDto>> Handle(DecideLoanRequestsCommand request, CancellationToken ct)
    {
        var approve = request.Decision == DecisionType.Approve;
        var key = DecideKey(request.Stage, approve);
        if (!Permissions.HasAnywhere(key)) throw FacilityDomainException.Forbidden("به این عملیات دسترسی ندارید.");
        var ids = request.RequestIds.Distinct().ToList();
        if (ids.Count == 0) throw Invalid("حداقل یک درخواست انتخاب کنید.");

        // دلیل‌ها به طرح تعلق دارند و یک دسته می‌تواند شرکت‌هایی از چند طرح داشته باشد، پس
        // دلیل بدون قید طرح خوانده و تطابقش برای هر ردیف بررسی می‌شود.
        Reason? rejectReason = null;
        var description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        if (!approve)
        {
            rejectReason = await ReasonAnySchemeAsync(request.ReasonId, ReasonType.LoanReject, "برای رد، انتخاب دلیل الزامی است.", ct);
            if (description == null) throw Invalid("برای رد، درج توضیحات الزامی است.");
        }
        // US-A6: تصمیم ادمین در «تأیید اعتبارات» و «تأیید شعبهٔ دیجیتال» با درج توضیحات و الصاق فایل اجباری است؛
        // برای سایر نقش‌ها اختیاری (قاعدهٔ فرایندی ۵). صف مدیر شرکت پیوست ندارد (Endpoint آن JSON است).
        if (IsSystemAdmin && request.Stage != ApprovalStage.Manager)
        {
            if (description == null) throw Invalid("برای تصمیم ادمین، درج توضیحات الزامی است.");
            if (request.Attachment == null) throw Invalid("برای تصمیم ادمین، الصاق فایل الزامی است.");
        }

        // مرحلهٔ پس از تأیید در همین تصمیم (درخواست فرانت): کلید جدا دارد و فقط در تأیید معنی دارد.
        Reason? postReason = null;
        if (request.PostStage is { } postStage)
        {
            if (!approve) throw Invalid("«مرحلهٔ پس از تأیید» فقط در تأیید قابل ثبت است.");
            if (!Permissions.HasAnywhere(K.CreditApprovalStage))
                throw FacilityDomainException.Forbidden("به ثبت «مرحلهٔ پس از تأیید» دسترسی ندارید.");
            if (postStage == PostApprovalStage.RemoteGrantDenied)
                postReason = await ReasonAnySchemeAsync(request.PostStageReasonId, ReasonType.RemoteGrantDenied,
                    "برای «عدم اعطای تسهیلات غیرحضوری» انتخاب دلیل الزامی است.", ct);
        }

        var loans = await Db.LoanRequests.Include(l => l.History).Where(l => ids.Contains(l.Id)).ToDictionaryAsync(l => l.Id, ct);
        var inScope = loans.Values.Where(l => CanOn(key, l)).ToList();
        var companyIds = inScope.Select(l => l.CompanyId).Distinct().ToList();
        var companies = await Db.Companies.Where(c => companyIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id, ct);
        var schemeIds = companies.Values.Select(c => c.SchemeId).Distinct().ToList();
        var digital = await Db.SchemeSettings.Where(x => schemeIds.Contains(x.SchemeId)).ToDictionaryAsync(x => x.SchemeId, x => x.DigitalApprovalEnabled, ct);

        StoredFile? attachment = null;
        if (request.Attachment != null)
            attachment = await StoreAsync(storage, request.Attachment, FileRules.LetterExtensions, FileRules.FiveMb, required: false, ct);

        var stageTitle = request.Stage switch { ApprovalStage.Manager => "مدیر شرکت", ApprovalStage.Credit => "تأیید اعتبارات", _ => "شعبهٔ دیجیتال" };
        var skipped = new List<SkippedRequestDto>();
        var processed = 0;
        foreach (var id in ids)
        {
            if (!loans.TryGetValue(id, out var loan) || !CanOn(key, loan)) { skipped.Add(new(id, "خارج از دامنهٔ شما")); continue; }
            var company = companies[loan.CompanyId];
            if (rejectReason != null && rejectReason.SchemeId != company.SchemeId)
            {
                skipped.Add(new(id, "دلیل انتخاب‌شده به طرح این شرکت تعلق ندارد"));
                continue;
            }
            if (postReason != null && postReason.SchemeId != company.SchemeId)
            {
                skipped.Add(new(id, "دلیل «عدم اعطای غیرحضوری» به طرح این شرکت تعلق ندارد"));
                continue;
            }
            var reasonTitle = rejectReason?.Title;
            var before = loan.Status;
            if (!loan.TryDecide(request.Stage, approve, digital.GetValueOrDefault(company.SchemeId), reasonTitle, description, Me, Now))
            {
                skipped.Add(new(id, "در این مرحله نیست"));
                continue;
            }
            if (attachment != null) loan.AttachDocument(attachment);
            // فقط درخواستی که با همین تصمیم تأییدشده شد؛ اگر تأیید شعبهٔ دیجیتال فعال باشد
            // وضعیت PendingDigital است و مرحله در تصمیم آن مرحله ثبت می‌شود.
            if (request.PostStage is { } stage && loan.Status == LoanRequestStatus.Approved && CanOn(K.CreditApprovalStage, loan))
                loan.SetPostStage(stage, postReason, description, Me, Now);
            processed++;
            Log(ChangeLogCategory.Loan, approve ? "تأیید درخواست" : "رد درخواست",
                $"{stageTitle}: درخواست {PersianText.Number(loan.Id)}" + (reasonTitle != null ? $" — {reasonTitle}" : ""), company,
                new { status = before }, new { status = loan.Status });
        }
        if (processed == 0 && attachment != null) Db.Files.Remove(attachment);
        await Db.SaveChangesAsync(ct);
        return Ok(new DecisionResultDto(processed, skipped),
            $"{PersianText.Number(processed)} درخواست {(approve ? "تأیید" : "رد")} شد" + (skipped.Count > 0 ? $"؛ {PersianText.Number(skipped.Count)} مورد انجام نشد." : "."));
    }
}
