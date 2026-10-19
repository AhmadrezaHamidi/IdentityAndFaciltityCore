using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Loans.Commands.CreateLoanRequests;

public sealed class CreateLoanRequestsCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : LoanHandlerBase(db, user, time), IRequestHandler<CreateLoanRequestsCommand, FacilityBaseMessageResponse<LoanBatchDto>>
{
    public async Task<FacilityBaseMessageResponse<LoanBatchDto>> Handle(CreateLoanRequestsCommand request, CancellationToken ct)
    {
        var company = await CompanyInScopeAsync(request.CompanyId, ct, K.LoansCreate);

        // قاعدهٔ ۹: تاریخ مبنا فقط در بازهٔ یک ماه گذشته
        if (request.BaseDate > Today || request.BaseDate < Jalali.AddMonths(Today, -1))
            throw Invalid("تاریخ مبنا فقط در بازهٔ یک ماه گذشته مجاز است.");
        var credit = await CreditAsync(company, request.BaseDate, ct);
        if (credit.Banner.Status != EligibilityStatus.Allowed) throw Invalid(credit.Banner.Message);
        var plan = credit.Plans.FirstOrDefault(p => p.Plan.Id == request.PlanId) ?? throw Invalid("ابتدا یکی از طرح‌های تفاهم‌نامه را انتخاب کنید.");

        if (request.Items.Count == 0) throw Invalid("حداقل یک نفر انتخاب کنید.");
        if (request.Items.Select(i => i.PersonnelId).Distinct().Count() != request.Items.Count) throw Invalid("هر نفر فقط یک بار قابل انتخاب است.");
        var amounts = request.Items.Select(i => request.Mode == AllocationMode.Single ? request.Amount ?? 0 : i.Amount ?? 0).ToList();
        if (amounts.Any(a => a <= 0)) throw Invalid("مبلغ وام برای همهٔ افراد انتخاب‌شده الزامی است.");
        var total = amounts.Sum();
        if (total > plan.RemainingThisMonth)
            throw Invalid($"مجموع مبالغ ({PersianText.Rial(total)}) بیشتر از اعتبار باقی‌ماندهٔ این طرح ({PersianText.Rial(plan.RemainingThisMonth)}) است.");

        var people = await EligiblePersonnelAsync(company.Id, request.Items.Select(i => i.PersonnelId).ToList(), ct);
        var batch = new LoanBatch(company.Id, company.BranchCode, plan.Plan.Id, request.Mode, request.BaseDate, request.Description, Me, Now);
        for (var i = 0; i < request.Items.Count; i++)
        {
            var person = people[request.Items[i].PersonnelId];
            batch.Add(person.Id, amounts[i], request.Items[i].PaymentOrder ?? person.Priority, Me, Now);
        }
        // قاعدهٔ فرایندی ۱: صف «تأیید مدیر شرکت» برای درخواست‌های منابع انسانی است. اگر ثبت‌کننده خودش تأییدکنندهٔ
        // این مرحله است (مدیر شرکت یا ادمین)، درخواست به صف تأیید خودش نمی‌رود و مستقیم به «تأیید اعتبارات» می‌رسد.
        if (CanOn(K.ManagerApprovalApprove, company))
            foreach (var r in batch.Requests)
                r.TryDecide(ApprovalStage.Manager, true, false, null, "ثبت توسط تأییدکنندهٔ مرحلهٔ مدیر شرکت", Me, Now);
        company.ReserveCredit();
        Db.LoanBatches.Add(batch);
        Log(ChangeLogCategory.Loan, "ثبت درخواست وام",
            $"{company.Name}: {PersianText.Number(request.Items.Count)} نفر، {plan.Plan.Title}، مجموع {PersianText.Rial(total)}", company,
            after: new { plan = plan.Plan.Id, total, people = request.Items.Count });
        await Db.SaveChangesAsync(ct);
        return Ok(await BatchDtoAsync(batch, ct));
    }
}
