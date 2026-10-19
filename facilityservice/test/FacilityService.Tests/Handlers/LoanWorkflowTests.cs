
namespace FacilityService.Tests.Handlers;

/// <summary>تخصیص وام و جریان تأیید — US-E4، E5، D2..D6، B7، B8 و قواعد فرایندی ۱ تا ۱۰.</summary>
public class LoanWorkflowTests
{
    [Fact]
    public async Task US_E4_credit_follows_prototype_formula_and_banner()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var credit = (await host.As("hr.alfa", new GetCompanyCreditQuery(host.Alfa, null))).Ok();

        // مجموع میانگین ۱۸ میلیارد × ضریب ۱٫۱ × سهم ½ × درصد طرح
        Assert.Equal(EligibilityStatus.Allowed, credit.Banner.Status);
        Assert.Equal(18_000_000_000, credit.TotalAverageBase);
        Assert.Equal(7_000_000_000, credit.CurrentMonthAverage);
        var qard = credit.Plans[0];
        Assert.Equal(792_000_000, qard.CreatedCredit);          // ۸٪
        Assert.Equal(90_000_000, qard.Allocated);               // علی ۵۰ + زهرا ۴۰ (در جریان)
        Assert.Equal(702_000_000, qard.RemainingThisMonth);
        Assert.Equal(990_000_000, credit.Plans[1].CreatedCredit); // مرابحه ۵٪، ۱۰٪ قابل استفاده
        Assert.Equal(702_000_000 + 990_000_000, credit.MonthRemainingCredit);

        var gamma = (await host.As("manager3", new GetCompanyCreditQuery(host.Gamma, null))).Ok();
        Assert.Equal(EligibilityStatus.NotTransferDay, gamma.Banner.Status);
        Assert.Contains("روز مجاز انتقال", gamma.Banner.Message);
        (await host.As("manager3", new GetCompanyCreditQuery(host.Alfa, null))).Fails(404);
    }

    [Fact]
    public async Task US_E5_and_D6_preview_hints_match_prototype()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var plan = await host.PlanIdAsync(ContractType.QardHasan, 4, 12);
        var q = new PreviewLoanRequestQuery { CompanyId = host.Alfa, PlanId = plan, Mode = AllocationMode.Single, Amount = 100_000_000 };

        var none = (await host.As("hr.alfa", q)).Ok();
        Assert.Equal(7, none.MaxCoverablePeople);  // ۷۰۲ میلیون ÷ ۱۰۰ میلیون
        Assert.Contains("حداکثر می‌توانید به ۷ نفر", none.Messages[0].Text);
        Assert.Contains("ده میلیون تومان", none.Messages[0].Text);

        var items = Enumerable.Range(1, 8).Select(i => new LoanRequestItemInput { PersonnelId = i }).ToList();
        var over = (await host.As("hr.alfa", q with { Items = items })).Ok();
        Assert.False(over.IsAllowed);
        Assert.Equal(HintLevel.Error, over.Messages[0].Level);
        Assert.Equal(87_750_000, over.SuggestedMaxAmount);

        var ok = (await host.As("hr.alfa", q with { Items = items.Take(2).ToList() })).Ok();
        Assert.True(ok.IsAllowed);
        Assert.Contains("مجاز است", ok.Messages[0].Text);

        var variable = (await host.As("hr.alfa", q with { Mode = AllocationMode.Variable, Items = [new() { PersonnelId = 1, Amount = 20_000_000 }, new() { PersonnelId = 2 }] })).Ok();
        Assert.Contains("مبلغ وام را", variable.Messages[0].Text);
    }

    [Fact]
    public async Task US_D3_create_requests_validates_people_credit_and_base_date()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var plan = await host.PlanIdAsync(ContractType.QardHasan, 4, 12);
        var cmd = new CreateLoanRequestsCommand
        {
            CompanyId = host.Alfa, PlanId = plan, BaseDate = FacilityTestHost.Today, Mode = AllocationMode.Variable,
            Items = [new() { PersonnelId = host.People["رضا کریمی"], Amount = 30_000_000 }, new() { PersonnelId = host.People["مریم حسینی"], Amount = 20_000_000 }],
        };

        (await host.As("hr.alfa", cmd with { BaseDate = FacilityTestHost.Today.AddDays(-40) })).Fails(400, "یک ماه");
        (await host.As("hr.alfa", cmd with { Items = [new() { PersonnelId = host.People["حسین رضایی"], Amount = 30_000_000 }] })).Fails(400, "قابل انتخاب");
        (await host.As("hr.alfa", cmd with { Items = [new() { PersonnelId = host.People["علی محمدی"], Amount = 30_000_000 }] })).Fails(400, "درخواست باز");
        (await host.As("hr.alfa", cmd with { Items = [new() { PersonnelId = host.People["رضا کریمی"], Amount = 800_000_000 }] })).Fails(400, "بیشتر از اعتبار");
        (await host.As("hr.alfa", cmd with { CompanyId = host.Gamma })).Fails(404);
        (await host.As("manager3", cmd with { CompanyId = host.Gamma, Items = [new() { PersonnelId = host.People["سارا جعفری"], Amount = 20_000_000 }] }))
            .Fails(400, "روز مجاز انتقال");

        var batch = (await host.As("hr.alfa", cmd)).Ok();
        Assert.Equal(50_000_000, batch.Total);
        Assert.All(batch.Requests, r => Assert.Equal(LoanRequestStatus.PendingManager, r.Status));
        Assert.Equal([3, 4], batch.Requests.Select(r => r.PaymentOrder)); // پیش‌فرض اولویت پرسنل
        Assert.Equal(652_000_000, (await host.As("hr.alfa", new GetCompanyCreditQuery(host.Alfa, null))).Ok().Plans[0].RemainingThisMonth);
    }

    [Fact]
    public async Task Q12_branch_user_does_not_see_requests_still_waiting_for_the_company_manager()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var ali = (await host.As("manager", new GetApprovalQueueQuery { Stage = ApprovalStage.Manager })).Ok().Items.Single();
        Assert.Equal(LoanRequestStatus.PendingManager, ali.Status);

        // کاربر شعبه: نه در لیست، نه جست‌وجوی مستقیم، نه جزئیات؛ فقط درخواست‌های مرحلهٔ اعتبارات به بعد
        var branchList = (await host.As("branch", new SearchLoanRequestsQuery())).Ok();
        Assert.DoesNotContain(branchList.Items, i => i.Id == ali.Id);
        Assert.Empty(branchList.Items.Where(i => i.Status is LoanRequestStatus.PendingManager or LoanRequestStatus.RejectedByManager));
        Assert.Contains(branchList.Items, i => i.PersonnelName == "زهرا احمدی");        // PendingCredit دیده می‌شود
        Assert.Empty((await host.As("branch", new SearchLoanRequestsQuery { Status = LoanRequestStatus.PendingManager })).Ok().Items);
        Assert.Empty((await host.As("branch", new SearchLoanRequestsQuery { Term = "علی محمدی" })).Ok().Items);
        (await host.As("branch", new GetLoanRequestQuery(ali.Id))).Fails(404);
        // فیلتر وضعیت صف هم نباید مرحلهٔ مدیر را نشان دهد
        (await host.As("branch", new GetApprovalQueueQuery { Stage = ApprovalStage.Credit, Status = LoanRequestStatus.PendingManager })).Fails(400, "صف");
        (await host.As("branch", new GetApprovalQueueQuery { Stage = ApprovalStage.Credit, Status = LoanRequestStatus.Cancelled })).Fails(400, "صف");

        // ثبت‌کننده (منابع انسانی)، مدیر شرکت، شعبهٔ دیجیتال و ادمین می‌بینند
        foreach (var who in new[] { "hr.alfa", "manager", "digital", "admin" })
        {
            Assert.Contains((await host.As(who, new SearchLoanRequestsQuery())).Ok().Items, i => i.Id == ali.Id);
            Assert.Equal(ali.Id, (await host.As(who, new GetLoanRequestQuery(ali.Id))).Ok().Summary.Id);
        }

        // بعد از تأیید مدیر، درخواست به شعبه می‌رسد و دیده می‌شود
        (await host.As("manager", new DecideLoanRequestsCommand { Stage = ApprovalStage.Manager, RequestIds = [ali.Id], Decision = DecisionType.Approve })).Ok();
        Assert.Contains((await host.As("branch", new SearchLoanRequestsQuery())).Ok().Items, i => i.Id == ali.Id);
        Assert.Equal(ali.Id, (await host.As("branch", new GetLoanRequestQuery(ali.Id))).Ok().Summary.Id);
    }

    [Fact]
    public async Task Full_workflow_manager_then_branch_credit_then_post_stage()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var ali = (await host.As("manager", new GetApprovalQueueQuery { Stage = ApprovalStage.Manager })).Ok().Items.Single();
        Assert.Equal("علی محمدی", ali.PersonnelName);
        Assert.Empty((await host.As("manager3", new GetApprovalQueueQuery { Stage = ApprovalStage.Manager })).Ok().Items);

        var r1 = (await host.As("manager", new DecideLoanRequestsCommand { Stage = ApprovalStage.Manager, RequestIds = [ali.Id], Decision = DecisionType.Approve })).Ok();
        Assert.Equal(1, r1.Processed);

        // US-B7: صف اعتبارات کاربر شعبه فقط شعبهٔ خودش (علی و زهرا، نه امیرِ گاما)
        var queue = (await host.As("branch", new GetApprovalQueueQuery { Stage = ApprovalStage.Credit, Status = LoanRequestStatus.PendingCredit })).Ok();
        Assert.Equal(["زهرا احمدی", "علی محمدی"], queue.Items.Select(i => i.PersonnelName).OrderBy(n => n));
        var gammaRequest = (await host.As("branch2", new GetApprovalQueueQuery { Stage = ApprovalStage.Credit })).Ok().Items.Single().Id;

        // US-B8: رد بدون دلیل/توضیح ممنوع؛ گروهی با شناسهٔ خارج از دامنه ← Skipped
        (await host.As("branch", new DecideLoanRequestsCommand { Stage = ApprovalStage.Credit, RequestIds = [ali.Id], Decision = DecisionType.Reject, ReasonId = await host.ReasonIdAsync(ReasonType.LoanReject) }))
            .Fails(400, "توضیحات");
        var decision = (await host.As("branch", new DecideLoanRequestsCommand
        {
            Stage = ApprovalStage.Credit, RequestIds = [ali.Id, gammaRequest, 999_999], Decision = DecisionType.Approve, Attachment = TestData.Pdf("approval.pdf"),
        })).Ok();
        Assert.Equal(1, decision.Processed);
        Assert.Equal(2, decision.Skipped.Count);

        var details = (await host.As("branch", new GetLoanRequestQuery(ali.Id))).Ok();
        Assert.Equal(LoanRequestStatus.Approved, details.Summary.Status);
        Assert.Equal([LoanRequestStatus.PendingManager, LoanRequestStatus.PendingCredit, LoanRequestStatus.Approved], details.History.Select(h => h.To));
        Assert.Equal("approval.pdf", Assert.Single(details.Documents).FileName);
        (await host.As("branch2", new GetLoanRequestQuery(ali.Id))).Fails(404);

        // مرحلهٔ پس از تأیید
        (await host.As("branch", new SetPostApprovalStageCommand { RequestId = ali.Id, Stage = PostApprovalStage.RemoteGrantDenied })).Fails(400, "دلیل");
        var staged = (await host.As("branch", new SetPostApprovalStageCommand { RequestId = ali.Id, Stage = PostApprovalStage.FileOpenedAtBranch })).Ok();
        Assert.Equal(PostApprovalStage.FileOpenedAtBranch, staged.PostStage);
        (await host.As("digital", new SetPostApprovalStageCommand { RequestId = ali.Id, Stage = PostApprovalStage.ReceivedAtBranch })).Fails(404);

        // دوباره تصمیم روی همان درخواست ← «در این مرحله نیست»
        var again = (await host.As("branch", new DecideLoanRequestsCommand { Stage = ApprovalStage.Credit, RequestIds = [ali.Id], Decision = DecisionType.Approve })).Ok();
        Assert.Equal("در این مرحله نیست", again.Skipped.Single().Reason);
    }

    [Fact]
    public async Task Group_reject_uses_one_reason_and_releases_credit()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var ids = (await host.As("admin", new GetApprovalQueueQuery { Stage = ApprovalStage.Credit, Status = LoanRequestStatus.PendingCredit })).Ok().Items.Select(i => i.Id).ToList();
        Assert.Equal(2, ids.Count);
        var r = (await host.As("admin", new DecideLoanRequestsCommand
        {
            Stage = ApprovalStage.Credit, RequestIds = ids, Decision = DecisionType.Reject, ReasonId = await host.ReasonIdAsync(ReasonType.LoanReject), Description = "سقف فردی رعایت نشده",
            Attachment = TestData.Pdf("reject.pdf"),
        })).Ok();
        Assert.Equal(2, r.Processed);
        var items = (await host.As("admin", new SearchLoanRequestsQuery { Status = LoanRequestStatus.RejectedByCredit })).Ok().Items;
        Assert.All(items, i => Assert.Equal("عدم کنترل سقف فردی — سقف فردی رعایت نشده", i.Description));
        Assert.Equal(50_000_000, (await host.As("admin", new GetCompanyCreditQuery(host.Alfa, null))).Ok().Plans[0].Allocated);
    }

    [Fact]
    public async Task US_A6_admin_decision_in_credit_stage_requires_description_and_file()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var zahra = (await host.As("admin", new GetApprovalQueueQuery { Stage = ApprovalStage.Credit, Status = LoanRequestStatus.PendingCredit })).Ok()
            .Items.Single(i => i.PersonnelName == "زهرا احمدی");
        var approve = new DecideLoanRequestsCommand { Stage = ApprovalStage.Credit, RequestIds = [zahra.Id], Decision = DecisionType.Approve };

        (await host.As("admin", approve with { Attachment = TestData.Pdf() })).Fails(400, "توضیحات");
        (await host.As("admin", approve with { Description = "مصوبهٔ کمیته" })).Fails(400, "الصاق فایل");
        Assert.Equal(1, (await host.As("admin", approve with { Description = "مصوبهٔ کمیته", Attachment = TestData.Pdf() })).Ok().Processed);

        // برای کاربر شعبه هر دو اختیاری است (قاعدهٔ فرایندی ۵)
        var ali = (await host.As("manager", new GetApprovalQueueQuery { Stage = ApprovalStage.Manager })).Ok().Items.Single();
        (await host.As("manager", new DecideLoanRequestsCommand { Stage = ApprovalStage.Manager, RequestIds = [ali.Id], Decision = DecisionType.Approve })).Ok();
        Assert.Equal(1, (await host.As("branch", new DecideLoanRequestsCommand { Stage = ApprovalStage.Credit, RequestIds = [ali.Id], Decision = DecisionType.Approve })).Ok().Processed);
    }

    [Fact]
    public async Task Requests_registered_by_the_company_manager_skip_the_manager_queue()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var plan = await host.PlanIdAsync(ContractType.QardHasan, 4, 12);
        var batch = (await host.As("manager", new CreateLoanRequestsCommand
        {
            CompanyId = host.Alfa, PlanId = plan, BaseDate = FacilityTestHost.Today, Mode = AllocationMode.Single, Amount = 25_000_000,
            Items = [new() { PersonnelId = host.People["رضا کریمی"] }],
        })).Ok();

        Assert.Equal(LoanRequestStatus.PendingCredit, batch.Requests.Single().Status);
        Assert.DoesNotContain((await host.As("manager", new GetApprovalQueueQuery { Stage = ApprovalStage.Manager })).Ok().Items, i => i.PersonnelName == "رضا کریمی");
        Assert.Contains((await host.As("branch", new GetApprovalQueueQuery { Stage = ApprovalStage.Credit, Status = LoanRequestStatus.PendingCredit })).Ok().Items,
            i => i.PersonnelName == "رضا کریمی");
    }

    [Fact]
    public async Task Digital_stage_is_used_only_when_enabled_in_basic_info()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var info = (await host.As("admin", new GetBasicInfoQuery(1))).Ok();
        (await host.As("admin", new UpdateBasicInfoCommand { SchemeId = 1, DigitalApprovalEnabled = true, RowVersion = info.RowVersion })).Ok();

        var zahra = (await host.As("branch", new GetApprovalQueueQuery { Stage = ApprovalStage.Credit, Status = LoanRequestStatus.PendingCredit })).Ok().Items.Single(i => i.PersonnelName == "زهرا احمدی");
        (await host.As("branch", new DecideLoanRequestsCommand { Stage = ApprovalStage.Credit, RequestIds = [zahra.Id], Decision = DecisionType.Approve })).Ok();
        var digitalQueue = (await host.As("digital", new GetApprovalQueueQuery { Stage = ApprovalStage.Digital })).Ok();
        Assert.Equal(zahra.Id, digitalQueue.Items.Single().Id);
        Assert.Equal(1, (await host.As("digital", new GetPendingCountsQuery())).Ok().DigitalApproval);

        (await host.As("digital", new DecideLoanRequestsCommand { Stage = ApprovalStage.Digital, RequestIds = [zahra.Id], Decision = DecisionType.Approve })).Ok();
        Assert.Equal(LoanRequestStatus.Approved, (await host.As("digital", new GetLoanRequestQuery(zahra.Id))).Ok().Summary.Status);
    }

    [Fact]
    public async Task US_D4_and_D5_remove_member_before_decision_and_cancel_before_credit()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var plan = await host.PlanIdAsync(ContractType.QardHasan, 4, 12);
        var batch = (await host.As("hr.alfa", new CreateLoanRequestsCommand
        {
            CompanyId = host.Alfa, PlanId = plan, BaseDate = FacilityTestHost.Today, Mode = AllocationMode.Single, Amount = 25_000_000,
            Items = [new() { PersonnelId = host.People["رضا کریمی"] }],
        })).Ok();

        var added = (await host.As("hr.alfa", new AddLoanBatchMemberCommand { BatchId = batch.BatchId, Item = new() { PersonnelId = host.People["مریم حسینی"] } })).Ok();
        Assert.Equal(2, added.Requests.Count);
        Assert.Equal(50_000_000, added.Total);

        var removed = (await host.As("hr.alfa", new RemoveLoanBatchMemberCommand(batch.BatchId, added.Requests[1].Id))).Ok();
        Assert.Equal(25_000_000, removed.Total);

        // لغو: فقط مدیر شرکت (منابع انسانی مجوز ندارد)، دلیل از فهرست، قبل از تصمیم اعتبارات
        var reason = await host.ReasonIdAsync(ReasonType.LoanCancel);
        var zahra = (await host.As("manager", new SearchLoanRequestsQuery { CompanyId = host.Alfa, Status = LoanRequestStatus.PendingCredit })).Ok().Items.Single();
        Assert.True(zahra.CanCancel);
        (await host.As("hr.alfa", new CancelLoanRequestCommand { Id = zahra.Id, ReasonId = reason, Description = "انصراف داد" })).Fails(404);
        var cancelled = (await host.As("manager", new CancelLoanRequestCommand { Id = zahra.Id, ReasonId = reason, Description = "انصراف داد" })).Ok();
        Assert.Equal(LoanRequestStatus.Cancelled, cancelled.Status);
        (await host.As("manager", new CancelLoanRequestCommand { Id = zahra.Id, ReasonId = reason, Description = "انصراف داد" })).Fails(400, "لغو");
    }

    [Fact]
    public async Task Pending_counts_are_scoped_and_null_without_permission()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var manager = (await host.As("manager", new GetPendingCountsQuery())).Ok();
        Assert.Equal(1, manager.ManagerApproval);
        Assert.Equal(1, manager.PersonnelApproval);
        Assert.Null(manager.CreditApproval);

        var branch = (await host.As("branch", new GetPendingCountsQuery())).Ok();
        Assert.Equal(1, branch.CreditApproval);
        Assert.Null(branch.ManagerApproval);
        Assert.Equal(2, (await host.As("admin", new GetPendingCountsQuery())).Ok().CreditApproval);
    }
}
