
namespace FacilityService.Tests.Handlers;

/// <summary>تب «پرسنل شرکت» — US-E1..E3، US-D1 روی Handlerهای واقعی.</summary>
public class PersonnelStoryTests
{
    private static CreatePersonnelCommand New(long companyId, string nationalCode = "1234567891") => new()
    {
        CompanyId = companyId, FullName = "کاوه یزدانی", NationalCode = nationalCode, Mobile = "09121112233",
        PersonnelCode = "P900", HireDate = new DateOnly(2024, 1, 1), Priority = 7,
    };

    [Fact]
    public async Task US_E1_hr_sees_only_own_company_and_unapproved_people_are_hidden_from_other_roles()
    {
        await using var host = await FacilityTestHost.CreateAsync();

        var mine = (await host.As("hr.alfa", new SearchPersonnelQuery { CompanyId = host.Alfa })).Ok();
        Assert.Equal(6, mine.TotalCount);
        Assert.Equal(1, mine.Items[0].Priority);
        (await host.As("hr.alfa", new SearchPersonnelQuery { CompanyId = host.Gamma })).Fails(404);

        // شعبهٔ دیجیتال فقط تأییدشده‌ها را می‌بیند (معیار پرسنل ۳)
        var digital = (await host.As("digital", new SearchPersonnelQuery { CompanyId = host.Alfa })).Ok();
        Assert.All(digital.Items, p => Assert.Equal(PersonnelStatus.Approved, p.Status));
        Assert.Equal(4, digital.TotalCount);
        (await host.As("digital", new GetPersonnelQuery(host.People["حسین رضایی"]))).Fails(404);
        (await host.As("manager3", new GetPersonnelQuery(host.People["علی محمدی"]))).Fails(404);
    }

    [Fact]
    public async Task US_E2_manual_introduction_validates_fields_and_duplicates()
    {
        await using var host = await FacilityTestHost.CreateAsync();

        var created = (await host.As("hr.alfa", New(host.Alfa))).Ok();
        Assert.Equal(PersonnelStatus.PendingManager, created.Status);
        Assert.Single(created.History);

        (await host.As("hr.alfa", New(host.Alfa))).Fails(400, "کد ملی");
        (await host.As("hr.alfa", New(host.Alfa, "1234567890"))).Fails(400, "کد ملی معتبر نیست"); // رقم کنترل نادرست (F-0.4.2)
        (await host.As("hr.alfa", New(host.Alfa, "1111111111"))).Fails(400, "کد ملی معتبر نیست"); // رقم کنترل درست ولی ارقام تکراری
        (await host.As("hr.alfa", New(host.Alfa, "2234567890") with { HireDate = FacilityTestHost.Today.AddDays(1) })).Fails(400, "آینده");
        (await host.As("hr.alfa", New(host.Alfa, "3234567891") with { FullName = "John" })).Fails(400, "فارسی");
        (await host.As("hr.alfa", New(host.Gamma, "4234567892"))).Fails(404);
        (await host.As("branch", New(host.Alfa, "5234567893"))).Fails(404);
    }

    [Fact]
    public async Task US_E2_csv_and_xlsx_import_report_errors_per_row()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var csv = "نام و نام خانوادگی,کد ملی,موبایل,کد پرسنلی,تاریخ استخدام,اولویت تخصیص\n" +
                  "کاوه یزدانی,۱۲۳۴۵۶۷۸۹۱,09121112233,P900,1403/02/15,1\n" +
                  "سمیرا رادان,123,09121112234,P901,1403/02/15,2\n" +
                  "پویا مرادی,1234567891,09121112235,P902,1499/99/99,3\n" +
                  "تکرار کاوه,1234567891,09121112236,P903,1403/02/15,4\n";
        var result = (await host.As("hr.alfa", new ImportPersonnelCommand { CompanyId = host.Alfa, File = TestData.Csv(csv) })).Ok();
        Assert.Equal(4, result.TotalRows);
        Assert.Equal(1, result.Accepted);
        Assert.Equal([3, 4, 5], result.Errors.Select(e => e.Row));
        Assert.Equal(new DateOnly(2024, 5, 4), (await host.As("hr.alfa", new SearchPersonnelQuery { CompanyId = host.Alfa, Term = "کاوه" })).Ok().Items.Single().HireDate);

        var again = (await host.As("hr.alfa", new GetPersonnelImportQuery(result.BatchId))).Ok();
        Assert.Equal(3, again.Errors.Count);

        var xlsx = XlsxBuilder.Build([
            ["نام و نام خانوادگی", "کد ملی", "موبایل", "کد پرسنلی", "تاریخ استخدام", "اولویت تخصیص"],
            ["لیلا صدری", "9876543210", "09351234567", "X1", "1402/11/01", "3"],
        ]);
        var fromExcel = (await host.As("hr.alfa", new ImportPersonnelCommand { CompanyId = host.Alfa, File = new(xlsx, "people.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") })).Ok();
        Assert.Equal(1, fromExcel.Accepted);

        (await host.As("hr.alfa", new ImportPersonnelCommand { CompanyId = host.Alfa, File = TestData.Pdf() })).Fails(400, "فرمت");
    }

    [Fact]
    public async Task US_E2_legacy_xls_import_reads_text_numbers_and_excel_dates()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        // Fixtures/personnel.xls با xlwt ساخته شده (BIFF8): متن فارسی، کد ملی عددی، تاریخ اکسل، یک ردیف با موبایل نامعتبر (کد ملی لیلا با رقم کنترل درست شد)
        var xls = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "personnel.xls"));
        var result = (await host.As("hr.alfa", new ImportPersonnelCommand { CompanyId = host.Alfa, File = new(xls, "personnel.xls", "application/vnd.ms-excel") })).Ok();
        Assert.Equal(3, result.TotalRows);
        Assert.Equal(2, result.Accepted);
        Assert.Equal(4, Assert.Single(result.Errors).Row);

        var bahram = (await host.As("hr.alfa", new SearchPersonnelQuery { CompanyId = host.Alfa, Term = "بهرام" })).Ok().Items.Single();
        Assert.Equal("9876543210", bahram.NationalCode);
        Assert.Equal(new DateOnly(2023, 5, 1), bahram.HireDate);
        Assert.Equal(4, bahram.Priority);
        Assert.Equal(Jalali.ToDate(1402, 11, 1), (await host.As("hr.alfa", new SearchPersonnelQuery { CompanyId = host.Alfa, Term = "لیلا" })).Ok().Items.Single().HireDate);

        (await host.As("hr.alfa", new ImportPersonnelCommand { CompanyId = host.Alfa, File = new(new byte[2048], "broken.xls", "application/vnd.ms-excel") })).Fails(400, "معتبر نیست");
    }

    [Fact]
    public async Task US_E3_rejected_person_is_fixed_and_resubmitted_and_locked_after_loan()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var rejected = (await host.As("hr.alfa", new GetPersonnelQuery(host.People["نرگس موسوی"]))).Ok();
        Assert.Equal(PersonnelStatus.Rejected, rejected.Status);

        var fixedPerson = (await host.As("hr.alfa", new UpdatePersonnelCommand
        {
            Id = rejected.Id, FullName = rejected.FullName, NationalCode = rejected.NationalCode, Mobile = "09129998877",
            PersonnelCode = rejected.PersonnelCode, HireDate = rejected.HireDate, Priority = rejected.Priority, RowVersion = rejected.RowVersion,
        })).Ok();
        Assert.Equal(PersonnelStatus.PendingManager, fixedPerson.Status);
        Assert.Equal("اصلاح و ارسال دوباره", fixedPerson.History[^1].Description);

        // «علی» در درخواست وام است ← ویرایش و حذف قفل
        var ali = (await host.As("hr.alfa", new GetPersonnelQuery(host.People["علی محمدی"]))).Ok();
        Assert.True(ali.IsLocked);
        (await host.As("hr.alfa", new DeletePersonnelCommand { Id = ali.Id, Description = "ترک خدمت کرده", File = TestData.Pdf() })).Fails(400, "درخواست وام");
    }

    [Fact]
    public async Task US_D1_manager_approves_and_rejects_in_group_only_within_own_company()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var pending = host.People["حسین رضایی"];
        var other = (await host.As("hr.alfa", New(host.Alfa))).Ok().Id;

        (await host.As("manager", new DecidePersonnelCommand { PersonnelIds = [pending], Decision = DecisionType.Reject, ReasonId = 9999 })).Fails(400, "دلیل");
        Assert.Equal(0, (await host.As("manager3", new DecidePersonnelCommand { PersonnelIds = [pending, other], Decision = DecisionType.Approve })).Ok());

        Assert.Equal(2, (await host.As("manager", new DecidePersonnelCommand { PersonnelIds = [pending, other, host.People["علی محمدی"]], Decision = DecisionType.Approve })).Ok());
        var p = (await host.As("digital", new GetPersonnelQuery(pending))).Ok();
        Assert.Equal(PersonnelStatus.Approved, p.Status);
        Assert.Equal(5, p.History[^1].By.PanelUserId);
    }

    [Fact]
    public async Task US_D1_reject_reason_and_description_are_optional()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var pending = host.People["حسین رضایی"];
        Assert.Equal(1, (await host.As("manager", new DecidePersonnelCommand { PersonnelIds = [pending], Decision = DecisionType.Reject })).Ok());
        var p = (await host.As("hr.alfa", new GetPersonnelQuery(pending))).Ok();
        Assert.Equal(PersonnelStatus.Rejected, p.Status);
    }

    [Fact]
    public async Task Delete_requires_letter_and_admin_can_restore_it_from_change_log()
    {
        await using var host = await FacilityTestHost.CreateAsync();
        var id = host.People["مریم حسینی"];
        (await host.As("hr.alfa", new DeletePersonnelCommand { Id = id, Description = "ترک خدمت کرده" })).Fails(400, "فایل");
        Assert.True((await host.As("hr.alfa", new DeletePersonnelCommand { Id = id, Description = "ترک خدمت کرده", File = TestData.Pdf() })).Ok());
        (await host.As("hr.alfa", new GetPersonnelQuery(id))).Fails(404);

        var log = (await host.As("admin", new SearchChangeLogsQuery { Term = "حذف پرسنل" })).Ok().Items.Single();
        Assert.False(log.CanRestore);                      // Q16: بازگردانی فعلاً ساخته نمی‌شود
        Assert.False((await host.As("manager", new SearchChangeLogsQuery { Term = "حذف پرسنل" })).Ok().Items.Single().CanRestore);

        (await host.As("admin", new RestoreChangeCommand { Id = log.Id, Description = "حذف اشتباه بود" })).Fails(400, "فعلاً فعال نیست");
        (await host.As("hr.alfa", new GetPersonnelQuery(id))).Fails(404);   // حذف همچنان پابرجاست
    }
}

/// <summary>ساخت xlsx حداقلی (SpreadsheetML) برای تست خوانندهٔ اکسل.</summary>
internal static class XlsxBuilder
{
    public static byte[] Build(string[][] rows)
    {
        using var ms = new MemoryStream();
        using (var zip = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, true))
        {
            void Add(string name, string xml)
            {
                using var w = new StreamWriter(zip.CreateEntry(name).Open());
                w.Write(xml);
            }
            Add("[Content_Types].xml", "<?xml version=\"1.0\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"/>");
            var strings = rows.SelectMany(r => r).Distinct().ToList();
            Add("xl/sharedStrings.xml", "<?xml version=\"1.0\"?><sst xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
                string.Concat(strings.Select(s => $"<si><t>{System.Security.SecurityElement.Escape(s)}</t></si>")) + "</sst>");
            var sheet = string.Concat(rows.Select((r, i) => $"<row r=\"{i + 1}\">" +
                string.Concat(r.Select((c, j) => $"<c r=\"{(char)('A' + j)}{i + 1}\" t=\"s\"><v>{strings.IndexOf(c)}</v></c>")) + "</row>"));
            Add("xl/worksheets/sheet1.xml", "<?xml version=\"1.0\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>" + sheet + "</sheetData></worksheet>");
        }
        return ms.ToArray();
    }
}
