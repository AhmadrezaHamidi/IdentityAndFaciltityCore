using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Loans.Commands.AddLoanBatchMember;

public sealed class AddLoanBatchMemberCommandHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : LoanHandlerBase(db, user, time), IRequestHandler<AddLoanBatchMemberCommand, FacilityBaseMessageResponse<LoanBatchDto>>
{
    public async Task<FacilityBaseMessageResponse<LoanBatchDto>> Handle(AddLoanBatchMemberCommand request, CancellationToken ct)
    {
        var batch = await Db.LoanBatches.Include(b => b.Requests).FirstOrDefaultAsync(b => b.Id == request.BatchId, ct)
                    ?? throw FacilityDomainException.NotFound("درخواست یافت نشد.");
        var company = await CompanyInScopeAsync(batch.CompanyId, ct, K.LoansMembersEdit);
        if (!batch.AcceptsNewMembers) throw Invalid("بعد از تأیید اعتبارات امکان افزودن فرد به این درخواست نیست.");
        if (batch.Requests.Any(r => r.PersonnelId == request.Item.PersonnelId && r.Status != LoanRequestStatus.Cancelled))
            throw Invalid("این فرد در همین درخواست وجود دارد.");

        // همان قواعد ثبت درخواست، روی تاریخ مبنای همین ثبت: بنر واجد شرایط بودن و اعتبار باقی‌مانده
        var credit = await CreditAsync(company, batch.BaseDate, ct);
        if (credit.Banner.Status is EligibilityStatus.ContractInactive or EligibilityStatus.IntroductionNotYetAllowed)
            throw Invalid(credit.Banner.Message);
        var plan = credit.Plans.FirstOrDefault(p => p.Plan.Id == batch.PlanId) ?? throw Invalid("طرح این درخواست دیگر در تفاهم‌نامهٔ شرکت نیست.");
        var amount = request.Item.Amount ?? batch.Requests.FirstOrDefault(r => r.Status != LoanRequestStatus.Cancelled)?.Amount
                     ?? throw Invalid("مبلغ وام را وارد کنید.");
        if (amount > plan.RemainingThisMonth) throw Invalid($"مبلغ بیشتر از اعتبار باقی‌ماندهٔ طرح ({PersianText.Rial(plan.RemainingThisMonth)}) است.");

        var person = (await EligiblePersonnelAsync(company.Id, [request.Item.PersonnelId], ct))[request.Item.PersonnelId];
        var added = batch.Add(person.Id, amount, request.Item.PaymentOrder ?? person.Priority, Me, Now);
        if (CanOn(K.ManagerApprovalApprove, company))
            added.TryDecide(ApprovalStage.Manager, true, false, null, "ثبت توسط تأییدکنندهٔ مرحلهٔ مدیر شرکت", Me, Now);
        company.ReserveCredit();
        Log(ChangeLogCategory.Loan, "افزودن فرد به درخواست", $"{company.Name}: {person.FullName} — {PersianText.Rial(amount)}", company);
        await Db.SaveChangesAsync(ct);
        return Ok(await BatchDtoAsync(batch, ct));
    }
}
