using System.Text;
using FacilityService.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Infrastructure.Seed;

/// <summary>شناسه‌های دادهٔ نمونه (برای تست‌ها و انتساب نقش‌های شرکت در Identity).</summary>
public sealed record FacilityPanelSampleIds(long Alfa, long Beta, long Gamma, IReadOnlyDictionary<string, long> People);

/// <summary>
/// دادهٔ نمونهٔ پنل (فقط توسعه و تست) — همان دادهٔ بک‌اند شبیه‌سازی‌شدهٔ فرانت (frontend/src/mock/state.ts):
///   شرکت آلفا و بتا در شعبهٔ ۱۵۰۰۰۲، گاما در ۱۵۰۰۰۳؛ قرارداد ۳ ماه پیش؛ روز افتتاح آلفا و بتا = امروز (مجاز)، گاما روز دیگر.
///   ۸ پرسنل (تأییدشده، در انتظار، ردشده) و ۳ درخواست وام (در انتظار مدیر، در انتظار اعتبارات).
/// ثبت‌کننده‌ها همان کاربران نمونهٔ Identity با همان شناسه‌اند (branch=3، branch2=4، manager=5، hr.alfa=6).
/// روی دیتابیس تازه شناسهٔ شرکت‌ها ۱، ۲ و ۳ می‌شود (کاربران manager و hr.alfa به شرکت ۱، manager3 به ۳).
/// </summary>
public static class FacilityPanelSampleData
{
    public static async Task<FacilityPanelSampleIds> SeedAsync(FacilityPanelDbContext db, DateOnly today, DateTime now,
        IFacilityFileStorage? files = null, CancellationToken ct = default)
    {
        var settings = await db.SchemeSettings.Include(s => s.Plans).SingleAsync(ct);
        var qard12 = settings.Plans.Single(p => p.ContractType == ContractType.QardHasan && p.TermMonths == 12).Id;
        var murabaha12 = settings.Plans.Single(p => p.ContractType == ContractType.Murabaha && p.TermMonths == 12 && p.Rate == 5).Id;
        var branchUser = new Registrar(3, "کارشناس شعبه بازار", "399", "شعبه بازار تهران", "150002");
        var branch2User = new Registrar(4, "کارشناس شعبه ونک", "402", "شعبه ونک", "150003");
        var manager = new Registrar(5, "مدیر شرکت آلفا", "501", "شعبه بازار تهران", "150002");
        var hr = new Registrar(6, "منابع انسانی آلفا", "502", "شعبه بازار تهران", "150002");

        var contract = Jalali.AddMonths(today, -3);
        var (cy, cm, cd) = Jalali.Of(contract);
        var otherDay = Jalali.ToDate(cy, cm, cd % 28 + 1);
        var months = Jalali.ContractMonths(contract);

        async Task<StoredFile?> Agreement(string company, Registrar by)
        {
            if (files == null) return null;
            var content = SamplePdf($"Tafahom-name {company}");
            var key = await files.PutAsync("agreement.pdf", "application/pdf", content, ct);
            var stored = new StoredFile("agreement.pdf", "application/pdf", content.LongLength, key, now, by.UserName);
            db.Files.Add(stored);
            return stored;
        }

        async Task<Company> NewCompany(string name, string nid, string branch, DateOnly opening, Registrar by, int[] plans, bool sepah, long[] averages)
        {
            var c = Company.Create(settings.SchemeId, nid, branch,
                new CompanyData(name, $"NM-{nid[..3]}", $"0100{nid[..6]}", contract, opening, sepah, plans), settings.MaxPlansPerCompany, by, now,
                await Agreement(nid, by));
            c.SetBalances(averages.Select((a, i) => (months[i], (long?)a)).ToList(), by, now, today);
            db.Companies.Add(c);
            return c;
        }

        var alfa = await NewCompany("شرکت آلفا", "10101010101", "150002", contract, branchUser, [qard12, murabaha12], true, [5_000_000_000, 6_000_000_000, 7_000_000_000]);
        var beta = await NewCompany("شرکت بتا", "20202020202", "150002", contract, branchUser, [qard12], false, [1_000_000_000]);
        var gamma = await NewCompany("شرکت گاما", "30303030303", "150003", otherDay, branch2User, [qard12], true, [3_000_000_000, 3_000_000_000]);
        await db.SaveChangesAsync(ct);

        var n = 100;
        Personnel Person(Company c, string name, string code, PersonnelStatus status, int priority, Registrar by)
        {
            n++;
            var p = Personnel.Create(c.Id, new PersonnelData(name, SampleNationalCode(n), $"0912{n}0000", code, new DateOnly(2023, 4, 10), priority).Normalize(today), by, now);
            if (status != PersonnelStatus.PendingManager)
                p.TryDecide(status == PersonnelStatus.Approved, status == PersonnelStatus.Rejected ? "اطلاعات نادرست" : null, null, manager, now);
            db.Personnel.Add(p);
            return p;
        }
        var people = new[]
        {
            Person(alfa, "علی محمدی", "P101", PersonnelStatus.Approved, 1, hr),
            Person(alfa, "زهرا احمدی", "P102", PersonnelStatus.Approved, 2, hr),
            Person(alfa, "رضا کریمی", "P103", PersonnelStatus.Approved, 3, hr),
            Person(alfa, "مریم حسینی", "P104", PersonnelStatus.Approved, 4, hr),
            Person(alfa, "حسین رضایی", "P105", PersonnelStatus.PendingManager, 5, hr),
            Person(alfa, "نرگس موسوی", "P106", PersonnelStatus.Rejected, 6, hr),
            Person(gamma, "امیر نوری", "G201", PersonnelStatus.Approved, 1, branch2User),
            Person(gamma, "سارا جعفری", "G202", PersonnelStatus.Approved, 2, branch2User),
        };
        await db.SaveChangesAsync(ct);

        // درخواست‌ها: علی در انتظار مدیر؛ زهرا و امیر در انتظار اعتبارات
        void Batch(Company c, Personnel p, long amount, bool managerApproved)
        {
            var b = new LoanBatch(c.Id, c.BranchCode, qard12, AllocationMode.Single, today, null, hr, now);
            var r = b.Add(p.Id, amount, 1, hr, now);
            if (managerApproved) r.TryDecide(ApprovalStage.Manager, true, false, null, null, manager, now);
            db.LoanBatches.Add(b);
        }
        Batch(alfa, people[0], 50_000_000, false);
        Batch(alfa, people[1], 40_000_000, true);
        Batch(gamma, people[6], 30_000_000, true);
        await db.SaveChangesAsync(ct);

        return new FacilityPanelSampleIds(alfa.Id, beta.Id, gamma.Id, people.ToDictionary(p => p.FullName, p => p.Id));
    }

    /// <summary>
    /// فقط اجرای محلی (تست‌ها صدا نمی‌زنند): بقیهٔ وضعیت‌ها تا هر صفحه داده داشته باشد —
    /// پرسنل شرکت بتا، و درخواست‌های تأییدشده (با مرحلهٔ پس از تأیید)، ردشده توسط مدیر، ردشده توسط اعتبارات و لغوشده.
    /// روی همان کارکنان تازه ثبت می‌شود تا با درخواست‌های SeedAsync تداخل نداشته باشد.
    /// </summary>
    public static async Task SeedShowcaseAsync(FacilityPanelDbContext db, FacilityPanelSampleIds ids, DateOnly today, DateTime now,
        CancellationToken ct = default)
    {
        var settings = await db.SchemeSettings.Include(s => s.Plans).SingleAsync(ct);
        var qard12 = settings.Plans.Single(p => p.ContractType == ContractType.QardHasan && p.TermMonths == 12).Id;
        var reasons = await db.Reasons.Where(r => r.IsActive).ToListAsync(ct);
        Reason ReasonOf(ReasonType type) => reasons.First(r => r.Type == type);

        var admin = new Registrar(1, "مدیر سامانه", "001", "ستاد مرکزی", "150000");
        var branchUser = new Registrar(3, "کارشناس شعبه بازار", "399", "شعبه بازار تهران", "150002");
        var branch2User = new Registrar(4, "کارشناس شعبه ونک", "402", "شعبه ونک", "150003");
        var manager = new Registrar(5, "مدیر شرکت آلفا", "501", "شعبه بازار تهران", "150002");
        var hr = new Registrar(6, "منابع انسانی آلفا", "502", "شعبه بازار تهران", "150002");
        var manager3 = new Registrar(7, "مدیر شرکت گاما", "503", "شعبه ونک", "150003");

        var n = 200;
        Personnel Person(long companyId, string name, string code, int priority, Registrar by, Registrar decider, bool? approve = true)
        {
            n++;
            var p = Personnel.Create(companyId, new PersonnelData(name, SampleNationalCode(n), $"0912{n}0000", code, new DateOnly(2022, 9, 1), priority).Normalize(today), by, now);
            if (approve != null) p.TryDecide(approve.Value, approve.Value ? null : "اطلاعات نادرست", null, decider, now);
            db.Personnel.Add(p);
            return p;
        }

        var kian = Person(ids.Alfa, "کیان صادقی", "P107", 7, hr, manager);
        var neda = Person(ids.Alfa, "ندا شریفی", "P108", 8, hr, manager);
        var bahar = Person(ids.Beta, "بهار عباسی", "B301", 1, branchUser, admin);
        var omid = Person(ids.Beta, "امید فرهادی", "B302", 2, branchUser, admin);
        Person(ids.Beta, "پویا رحیمی", "B303", 3, branchUser, admin, approve: null);
        var hamed = Person(ids.Gamma, "حامد قاسمی", "G203", 3, branch2User, manager3);
        await db.SaveChangesAsync(ct);

        LoanRequest Request(long companyId, string branchCode, Personnel p, long amount, Registrar by)
        {
            var b = new LoanBatch(companyId, branchCode, qard12, AllocationMode.Single, today, null, by, now);
            db.LoanBatches.Add(b);
            return b.Add(p.Id, amount, 1, by, now);
        }

        // آلفا: تأییدشده با مرحلهٔ «تشکیل پرونده در شعبه»؛ لغوشده توسط مدیر شرکت
        var approved = Request(ids.Alfa, "150002", kian, 35_000_000, hr);
        approved.TryDecide(ApprovalStage.Manager, true, false, null, null, manager, now);
        approved.TryDecide(ApprovalStage.Credit, true, false, null, null, branchUser, now);
        approved.SetPostStage(PostApprovalStage.FileOpenedAtBranch, null, null, branchUser, now);
        Request(ids.Alfa, "150002", neda, 20_000_000, hr)
            .Cancel(ReasonOf(ReasonType.LoanCancel), "انصراف کارمند از دریافت وام", manager, now);

        // بتا: تأییدشده (منتظر مرحلهٔ پس از تأیید) و ردشده توسط اعتبارات
        var betaApproved = Request(ids.Beta, "150002", bahar, 25_000_000, admin);
        betaApproved.TryDecide(ApprovalStage.Manager, true, false, null, null, admin, now);
        betaApproved.TryDecide(ApprovalStage.Credit, true, false, null, null, branchUser, now);
        var betaRejected = Request(ids.Beta, "150002", omid, 45_000_000, admin);
        betaRejected.TryDecide(ApprovalStage.Manager, true, false, null, null, admin, now);
        betaRejected.TryDecide(ApprovalStage.Credit, false, false, ReasonOf(ReasonType.LoanReject).Title, "سقف اعتبار شرکت کافی نیست", branchUser, now);

        // گاما: ردشده توسط مدیر شرکت
        Request(ids.Gamma, "150003", hamed, 15_000_000, manager3)
            .TryDecide(ApprovalStage.Manager, false, false, ReasonOf(ReasonType.LoanReject).Title, "سابقهٔ کار کافی نیست", manager3, now);

        // گزارش تغییرات (Seed از Handlerها رد نمی‌شود، پس چند ردیف نمونه با همان عنوان‌های Handlerها)
        var companies = await db.Companies.ToDictionaryAsync(c => c.Id, ct);
        (Registrar By, ChangeLogCategory Category, string Action, string Details, long CompanyId)[] logs =
        [
            (branchUser, ChangeLogCategory.Company, "ثبت شرکت", "شرکت آلفا", ids.Alfa),
            (branchUser, ChangeLogCategory.Company, "ثبت شرکت", "شرکت بتا", ids.Beta),
            (branch2User, ChangeLogCategory.Company, "ثبت شرکت", "شرکت گاما", ids.Gamma),
            (branchUser, ChangeLogCategory.Company, "ثبت میانگین موجودی", "شرکت آلفا — ۳ ماه", ids.Alfa),
            (hr, ChangeLogCategory.Personnel, "معرفی پرسنل", "کیان صادقی (P107)", ids.Alfa),
            (branchUser, ChangeLogCategory.Personnel, "معرفی پرسنل", "پویا رحیمی (B303)", ids.Beta),
            (hr, ChangeLogCategory.Loan, "ثبت درخواست وام", "کیان صادقی — ۳۵٬۰۰۰٬۰۰۰ ریال", ids.Alfa),
            (branchUser, ChangeLogCategory.Loan, "تغییر مرحلهٔ پس از تأیید", "کیان صادقی — تشکیل پرونده در شعبه", ids.Alfa),
            (manager, ChangeLogCategory.Loan, "لغو درخواست وام", "ندا شریفی — انصراف کارمند از دریافت وام", ids.Alfa),
        ];
        for (var i = 0; i < logs.Length; i++)
            db.ChangeLogs.Add(new ChangeLog(now.AddMinutes(i - logs.Length), logs[i].By, logs[i].Category, logs[i].Action, logs[i].Details,
                null, null, companies[logs[i].CompanyId]));

        await db.SaveChangesAsync(ct);
    }

    /// <summary>یک PDF یک‌صفحه‌ای معتبر (برای فایل‌های نمونه در Minio).</summary>
    public static byte[] SamplePdf(string text)
    {
        var stream = $"BT /F1 18 Tf 72 720 Td ({text}) Tj ET";
        string[] objects =
        [
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>",
            $"<< /Length {stream.Length} >>\nstream\n{stream}\nendstream",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
        ];
        var sb = new StringBuilder("%PDF-1.4\n");
        var offsets = new List<int>();
        for (var i = 0; i < objects.Length; i++)
        {
            offsets.Add(Encoding.ASCII.GetByteCount(sb.ToString()));
            sb.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n");
        }
        var xref = Encoding.ASCII.GetByteCount(sb.ToString());
        sb.Append($"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n");
        foreach (var o in offsets) sb.Append($"{o:D10} 00000 n \n");
        sb.Append($"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
        return Encoding.ASCII.GetBytes(sb.ToString());
    }

    /// <summary>کد ملی نمونهٔ معتبر (با رقم کنترل): 00 + n + 1234 + رقم کنترل.</summary>
    private static string SampleNationalCode(int n)
    {
        var firstNine = $"00{n}1234";
        return firstNine + Domain.Helpers.NationalCode.CheckDigit(firstNine);
    }
}
