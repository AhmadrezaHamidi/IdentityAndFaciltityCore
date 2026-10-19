using System.Net;
using System.Reflection;
using System.Text;
using FacilityService.Infrastructure;
using FacilityService.Infrastructure.DBContext;
using FacilityService.Infrastructure.Seed;
using FacilityService.Infrastructure.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FacilityErDoc;

/// <summary>
/// سند Entityهای Facility (facilityservice/docs/Facility-ER.md و .html) مستقیم از مدل EF ساخته می‌شود:
/// جدول، ستون، نوع SQL، کلیدها، ایندکس‌ها و کلیدهای خارجی از مدل؛ توضیح فارسی از HasComment (FacilityPanelComments).
/// فقط دسته‌بندی، معنی هر کلید خارجی و بخش‌های «رابطهٔ بدون FK» و «قواعد Domain» اینجا نوشته شده‌اند.
/// </summary>
public sealed record FacilityErDocument(string Markdown, string Html, int TableCount, int ColumnCount, int ForeignKeyCount)
{
    public const string MarkdownPath = "facilityservice/docs/Facility-ER.md";
    public const string HtmlPath = "facilityservice/docs/Facility-ER.html";

    private static readonly (string Key, string Title, string Description, string[] Tables)[] Groups =
    [
        ("base", "اطلاعات پایه", "صفحهٔ اول نمونهٔ پنل", ["Schemes", "SchemeSettings", "SchemePlans"]),
        ("company", "شرکت", "تب مدیریت شرکت‌ها", ["Companies", "CompanyPlans", "MonthlyBalances", "CompanyCancellations", "BalanceDocuments"]),
        ("personnel", "پرسنل", "تب پرسنل شرکت", ["Personnel", "PersonnelHistory", "PersonnelImports"]),
        ("loan", "وام", "تخصیص وام و تأییدها", ["LoanBatches", "LoanRequests", "LoanRequestHistory", "LoanRequestDocuments"]),
        ("common", "مشترک", "فایل‌ها، دلایل و گزارش تغییرات", ["Reasons", "Files", "ChangeLogs"]),
    ];

    /// <summary>«جدول فرزند.ستون» ← معنی کسب‌وکاری رابطه.</summary>
    private static readonly Dictionary<string, string> ForeignKeyMeaning = new()
    {
        ["SchemeSettings.SchemeId"] = "هر طرح یک فرم اطلاعات پایه",
        ["SchemePlans.SchemeId"] = "طرح‌های جدول درصد قابل استفاده",
        ["Companies.SchemeId"] = "شرکت عضو یک طرح کلان است",
        ["Companies.AgreementFileId"] = "فایل تفاهم‌نامه",
        ["CompanyPlans.CompanyId"] = "طرح‌های تفاهم‌نامهٔ شرکت (۱ تا سقف)",
        ["CompanyPlans.PlanId"] = "شرکت ↔ طرح: چند به چند",
        ["MonthlyBalances.CompanyId"] = "یک ردیف برای هر ماه",
        ["CompanyCancellations.CompanyId"] = "لغو قرارداد",
        ["CompanyCancellations.ReasonId"] = "دلیل لغو (نوع ۱)",
        ["CompanyCancellations.LetterFileId"] = "نامهٔ لغو",
        ["BalanceDocuments.CompanyId"] = "مستندات میانگین",
        ["BalanceDocuments.DocumentTypeId"] = "نوع مستند (نوع ۶)",
        ["BalanceDocuments.FileId"] = "فایل مستند",
        ["Personnel.CompanyId"] = "پرسنل شرکت",
        ["Personnel.ImportBatchId"] = "فردی که از فایل اکسل آمده",
        ["Personnel.DeleteFileId"] = "فایل مستند حذف",
        ["PersonnelHistory.PersonnelId"] = "تاریخچهٔ وضعیت",
        ["PersonnelImports.CompanyId"] = "بارگذاری‌های گروهی",
        ["LoanBatches.CompanyId"] = "ثبت‌های تخصیص شرکت",
        ["LoanBatches.PlanId"] = "هر ثبت یک طرح",
        ["LoanRequests.BatchId"] = "یک درخواست برای هر نفر",
        ["LoanRequests.PersonnelId"] = "وام‌های یک نفر (حداکثر یک درخواست باز)",
        ["LoanRequests.CompanyId"] = "کپی شرکت ثبت",
        ["LoanRequests.PlanId"] = "کپی طرح ثبت",
        ["LoanRequestHistory.RequestId"] = "تاریخچهٔ تصمیم‌ها",
        ["LoanRequestDocuments.RequestId"] = "پیوست‌های تصمیم",
        ["LoanRequestDocuments.FileId"] = "یک فایل برای چند درخواست گروهی",
        ["ChangeLogs.CompanyId"] = "گزارش تغییرات شرکت",
    };

    private static readonly (string Column, string Target, string Why)[] LogicalRelations =
    [
        ("LoanRequests.BranchCode / ChangeLogs.BranchCode", "Companies.BranchCode", "کپی عمدی؛ شعبهٔ شرکت عوض نمی‌شود و فیلتر دامنهٔ شعبه بدون Join اعمال می‌شود."),
        ("LoanRequests.CompanyId / PlanId", "LoanBatches.CompanyId / PlanId", "کپی از ثبت."),
        ("ChangeLogs.UserId و همهٔ Registrar / By / EditedBy", "PanelUser در Identity", "سرویس دیگر؛ FK ندارد. کاربر به‌صورت عکس فوری در لحظهٔ عملیات ذخیره می‌شود."),
        ("ScopeId دامنهٔ «شرکت» در Identity", "Companies.Id", "نقش‌های مدیر شرکت و منابع انسانی به همین شناسه محدود می‌شوند."),
        ("ScopeId دامنهٔ «شعبه» در Identity", "Companies.BranchCode", "کاربر شعبه فقط شرکت‌های همین کد شعبه را می‌بیند."),
        ("CompanyCancellations.ReasonTitle، PersonnelHistory.Reason، LoanRequestHistory.Reason", "Reasons.Title", "عنوان دلیل در لحظهٔ عملیات کپی می‌شود؛ دلیل‌ها فقط غیرفعال می‌شوند، پس سوابق قدیمی سالم می‌مانند."),
    ];

    private static readonly string[] DomainRules =
    [
        "هر نفر حداکثر یک درخواست باز دارد (وضعیت ۱، ۳ یا ۵).",
        "فردی که در درخواست وامِ لغونشده آمده، قابل ویرایش و حذف نیست.",
        "طرحی که درخواست فعال دارد، از تفاهم‌نامهٔ شرکت حذف نمی‌شود.",
        "شرکتی که درخواست تأییدشده دارد، حذف نمی‌شود.",
        "«اعتبار ایجادشده» و «مانده» ذخیره نمی‌شوند؛ هر بار از میانگین‌ها، طرح‌ها و درخواست‌های ردنشده محاسبه می‌شوند (CreditRules).",
        "تعداد طرح‌های هر شرکت بین ۱ و MaxPlansPerCompany است.",
    ];

    // ── مدل ──

    private sealed record Column(string Name, string SqlType, string Keys, bool Nullable, string Comment);

    private sealed record Table(string Name, string Group, string Title, string Description, IReadOnlyList<Column> Columns);

    private sealed record Relation(string Child, string ChildColumn, string Parent, string Mermaid, string Cardinality, bool Required, string OnDelete, string Meaning);

    public static FacilityErDocument FromModel()
    {
        using var db = new FacilityPanelDbContext(new DbContextOptionsBuilder<FacilityPanelDbContext>()
            .UseSqlServer("Server=.;Database=Facility;Trusted_Connection=True", FacilityPanelModel.SqlServer).Options);
        return Build(db.GetService<IDesignTimeModel>().Model);
    }

    public static FacilityErDocument Build(IModel model)
    {
        var entities = model.GetEntityTypes().Where(e => e.GetTableName() != null && !e.IsOwned()).ToDictionary(e => e.GetTableName()!);
        var missing = new List<string>();
        var tables = new List<Table>();
        foreach (var (key, _, _, names) in Groups)
        foreach (var name in names)
        {
            if (!entities.TryGetValue(name, out var entity)) { missing.Add($"جدول {name} در مدل نیست"); continue; }
            var (title, description) = SplitComment(entity.GetComment(), name, missing);
            var store = StoreObjectIdentifier.Table(name, entity.GetSchema());
            var columns = OrderedProperties(entity).Select(p =>
            {
                var column = p.GetColumnName(store) ?? p.Name;
                if (string.IsNullOrWhiteSpace(p.GetComment())) missing.Add($"ستون {name}.{column} توضیح (HasComment) ندارد");
                return new Column(column, p.GetColumnType(store) ?? "?", KeysOf(entity, p), p.IsColumnNullable(store), p.GetComment() ?? "");
            }).ToList();
            tables.Add(new Table(name, key, title, description, columns));
        }
        missing.AddRange(entities.Keys.Where(t => !Groups.Any(g => g.Tables.Contains(t))).Select(t => $"جدول {t} در هیچ دسته‌ای نیست"));

        var relations = new List<Relation>();
        foreach (var table in tables)
        foreach (var fk in entities[table.Name].GetForeignKeys())
        {
            var parent = fk.PrincipalEntityType.GetTableName()!;
            var store = StoreObjectIdentifier.Table(table.Name, entities[table.Name].GetSchema());
            var column = string.Join(", ", fk.Properties.Select(p => p.GetColumnName(store)));
            if (!ForeignKeyMeaning.TryGetValue($"{table.Name}.{column}", out var meaning)) missing.Add($"کلید خارجی {table.Name}.{column} معنی ندارد");
            var principalSide = fk.IsRequired ? "||" : "|o";
            var dependentSide = fk.IsUnique ? (fk.IsRequiredDependent ? "||" : "o|") : "o{";
            var cardinality = fk.IsUnique ? (fk.IsRequiredDependent ? "۱ : ۱" : "۱ : ۰..۱") : "۱ : N";
            if (!fk.IsRequired) cardinality = "۰..۱" + cardinality[1..];
            relations.Add(new Relation(table.Name, column, parent, $"{parent} {principalSide}--{dependentSide} {table.Name}", cardinality,
                fk.IsRequired, fk.DeleteBehavior == DeleteBehavior.Cascade ? "Cascade" : "Restrict", meaning ?? ""));
        }
        missing.AddRange(ForeignKeyMeaning.Keys.Where(k => !relations.Any(r => $"{r.Child}.{r.ChildColumn}" == k)).Select(k => $"معنی برای کلید خارجی ناموجود {k}"));
        if (missing.Count > 0) throw new InvalidOperationException("سند Entityها کامل نیست:\n" + string.Join("\n", missing));

        var mermaid = Mermaid(tables, relations);
        var columnCount = tables.Sum(t => t.Columns.Count);
        return new FacilityErDocument(RenderMarkdown(tables, relations, mermaid, columnCount), RenderHtml(tables, relations, mermaid, columnCount),
            tables.Count, columnCount, relations.Count);
    }

    /// <summary>ترتیب تعریف در کلاس Domain (نه الفبایی EF)؛ ویژگی‌های کلاس پایه (Version) آخر.</summary>
    private static IEnumerable<IProperty> OrderedProperties(IEntityType entity)
    {
        var order = new List<string>();
        for (var t = entity.ClrType; t != null && t != typeof(object); t = t.BaseType)
            order.InsertRange(order.Count, t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).OrderBy(p => p.MetadataToken).Select(p => p.Name));
        return entity.GetProperties().OrderBy(p => order.IndexOf(p.Name) is var i and >= 0 ? i : int.MaxValue).ThenBy(p => p.Name);
    }

    private static string KeysOf(IEntityType entity, IProperty p)
    {
        var keys = new List<string>();
        if (p.IsPrimaryKey()) keys.Add("PK");
        if (p.IsForeignKey()) keys.Add("FK");
        var indexes = entity.GetIndexes().Where(i => i.Properties.Contains(p)).ToList();
        if (indexes.Any(i => i.IsUnique) && !p.IsPrimaryKey()) keys.Add("UK");
        else if (!p.IsPrimaryKey() && !p.IsForeignKey() && indexes.Count > 0) keys.Add("IX");
        return string.Join(", ", keys);
    }

    private static (string, string) SplitComment(string? comment, string table, List<string> missing)
    {
        var parts = (comment ?? "").Split(" — ", 2);
        if (parts.Length < 2) { missing.Add($"جدول {table} توضیح (عنوان — توضیح) ندارد"); return (table, ""); }
        return (parts[0], parts[1]);
    }

    private static string Mermaid(IReadOnlyList<Table> tables, IReadOnlyList<Relation> relations)
    {
        var sb = new StringBuilder("erDiagram\n");
        foreach (var r in relations) sb.Append($"  {r.Mermaid} : \"{r.ChildColumn}\"\n");
        foreach (var t in tables)
        {
            sb.Append($"  {t.Name} {{\n");
            foreach (var c in t.Columns)
            {
                var keys = string.Join(",", c.Keys.Split(", ", StringSplitOptions.RemoveEmptyEntries).Where(k => k is "PK" or "FK" or "UK"));
                var comment = c.Comment.Replace('"', '\'');
                if (comment.Length > 48) comment = comment[..46] + "…";
                sb.Append($"    {MermaidType(c.SqlType)} {c.Name}{(keys.Length > 0 ? " " + keys : "")} \"{comment}\"\n");
            }
            sb.Append("  }\n");
        }
        return sb.ToString().TrimEnd();
    }

    private static string MermaidType(string sql) => sql.Split('(')[0] switch
    {
        "nvarchar" or "varchar" => "string",
        "datetime2" => "datetime",
        "tinyint" => "byte",
        "bigint" => "long",
        "bit" => "bool",
        var other => other,
    };

    // ── Markdown ──

    private static string RenderMarkdown(IReadOnlyList<Table> tables, IReadOnlyList<Relation> relations, string mermaid, int columnCount)
    {
        string Md(string s) => s.Replace("|", "/");
        var md = new StringBuilder();
        md.AppendLine("# نقشهٔ دادهٔ پنل نگین میثاق (Facility)").AppendLine()
          .AppendLine($"همهٔ {tables.Count} جدول سرویس Facility (اسکیمای `panel`) با {columnCount} فیلد و {relations.Count} کلید خارجی؛ ساخته‌شده مستقیم از مدل EF.")
          .AppendLine("نسخهٔ HTML همین سند: [`Facility-ER.html`](Facility-ER.html). تولید دوباره: `dotnet run --project facilityservice/tools/FacilityErDoc` (تست `FacilityErDocumentTests` کهنه بودنش را می‌گیرد).").AppendLine()
          .AppendLine("**قراردادها:** مبلغ‌ها ریال؛ تاریخ‌ها میلادی ذخیره و شمسی نمایش؛ نرخ و درصد به درصد (۴ یعنی ۴٪). ستون‌های Registrar / By / EditedBy عکس فوری کاربر در لحظهٔ عملیات‌اند (JSON). توضیح هر جدول و ستون در SQL Server هم ثبت شده است (MS_Description).").AppendLine()
          .AppendLine("## نمودار کامل (ER)").AppendLine().AppendLine("```mermaid").AppendLine(mermaid).AppendLine("```").AppendLine();
        foreach (var (key, title, description, _) in Groups)
        {
            md.AppendLine($"## {title}").AppendLine().AppendLine(description).AppendLine();
            foreach (var t in tables.Where(t => t.Group == key))
            {
                md.AppendLine($"### `{t.Name}` — {t.Title}").AppendLine().AppendLine(t.Description).AppendLine()
                  .AppendLine("| فیلد | نوع SQL | کلید | Null | توضیح |").AppendLine("|---|---|---|---|---|");
                foreach (var c in t.Columns) md.AppendLine($"| `{c.Name}` | `{c.SqlType}` | {c.Keys} | {(c.Nullable ? "✓" : "")} | {Md(c.Comment)} |");
                var outgoing = relations.Where(r => r.Child == t.Name).ToList();
                var incoming = relations.Where(r => r.Parent == t.Name).ToList();
                if (outgoing.Count > 0) md.AppendLine().AppendLine("**مرجع:** " + string.Join("، ", outgoing.Select(r => $"`{r.ChildColumn}` → `{r.Parent}`")));
                if (incoming.Count > 0) md.AppendLine().AppendLine("**ارجاع‌دهنده:** " + string.Join("، ", incoming.Select(r => $"`{r.Child}`")));
                md.AppendLine();
            }
        }
        md.AppendLine("## همهٔ کلیدهای خارجی").AppendLine()
          .AppendLine("**Cascade**: فرزند با والد حذف می‌شود. **Restrict**: والد تا فرزند دارد حذف نمی‌شود (شرکت و پرسنل حذف نرم می‌شوند).").AppendLine()
          .AppendLine("| ستون فرزند | جدول والد | نسبت | اجباری | حذف | معنی |").AppendLine("|---|---|---|---|---|---|");
        foreach (var r in relations) md.AppendLine($"| `{r.Child}.{r.ChildColumn}` | `{r.Parent}` | {r.Cardinality} | {(r.Required ? "✓" : "—")} | {r.OnDelete} | {r.Meaning} |");
        md.AppendLine().AppendLine("## رابطه‌های بدون کلید خارجی").AppendLine().AppendLine("| ستون | به | چرا |").AppendLine("|---|---|---|");
        foreach (var (column, target, why) in LogicalRelations) md.AppendLine($"| `{column}` | `{target}` | {why} |");
        md.AppendLine().AppendLine("## قواعدی که در Domain کنترل می‌شوند").AppendLine();
        foreach (var rule in DomainRules) md.AppendLine($"- {rule}");
        return md.ToString().Replace("\r\n", "\n");
    }

    // ── HTML ──

    private static string E(string s) => WebUtility.HtmlEncode(s);

    private static string RenderHtml(IReadOnlyList<Table> tables, IReadOnlyList<Relation> relations, string mermaid, int columnCount)
    {
        using var stream = typeof(FacilityErDocument).Assembly.GetManifestResourceStream("FacilityErDoc.template.html")!;
        var template = new StreamReader(stream).ReadToEnd();

        var nav = string.Concat(tables.Select(t => $"<a href=\"#{t.Name}\" class=\"chip g-{t.Group}\">{t.Name}</a>"));
        var cards = new StringBuilder();
        foreach (var (key, title, description, _) in Groups)
        {
            cards.Append($"<section class=\"group\" id=\"grp-{key}\"><h2><span class=\"dot g-{key}\"></span>{title}</h2><p class=\"lede\">{description}</p><div class=\"cards\">");
            foreach (var t in tables.Where(t => t.Group == key))
            {
                var rows = string.Concat(t.Columns.Select(c =>
                    $"<tr><td class=\"f\"><code>{c.Name}</code></td><td class=\"t\"><code>{E(c.SqlType)}</code></td>" +
                    $"<td class=\"k\">{string.Concat(c.Keys.Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(k => $"<span class=\"key {k.ToLowerInvariant()}\">{k}</span>"))}</td>" +
                    $"<td class=\"n\">{(c.Nullable ? "✓" : "")}</td><td class=\"d\">{E(c.Comment)}</td></tr>"));
                var outgoing = relations.Where(r => r.Child == t.Name).ToList();
                var incoming = relations.Where(r => r.Parent == t.Name).ToList();
                var rel = "";
                if (outgoing.Count > 0) rel += "<p class=\"rel\"><b>مرجع:</b> " + string.Join("، ", outgoing.Select(r => $"<code>{r.ChildColumn}</code> → <a href=\"#{r.Parent}\">{r.Parent}</a>")) + "</p>";
                if (incoming.Count > 0) rel += "<p class=\"rel\"><b>ارجاع‌دهنده:</b> " + string.Join("، ", incoming.Select(r => $"<a href=\"#{r.Child}\">{r.Child}</a>")) + "</p>";
                cards.Append($"<article class=\"card\" id=\"{t.Name}\"><header><h3 dir=\"ltr\">{t.Name}</h3><span class=\"fa\">{E(t.Title)}</span></header><p class=\"desc\">{E(t.Description)}</p>" +
                             $"<div class=\"tw\"><table><thead><tr><th>فیلد</th><th>نوع SQL</th><th>کلید</th><th>Null</th><th>توضیح</th></tr></thead><tbody>{rows}</tbody></table></div>{rel}</article>");
            }
            cards.Append("</div></section>");
        }
        var fk = string.Concat(relations.Select(r =>
            $"<tr><td><a href=\"#{r.Child}\">{r.Child}</a>.<code>{r.ChildColumn}</code></td><td><a href=\"#{r.Parent}\">{r.Parent}</a></td><td class=\"c\">{r.Cardinality}</td>" +
            $"<td class=\"c\">{(r.Required ? "✓" : "—")}</td><td><span class=\"del {r.OnDelete.ToLowerInvariant()}\">{r.OnDelete}</span></td><td>{E(r.Meaning)}</td></tr>"));
        var logical = string.Concat(LogicalRelations.Select(l => $"<tr><td><code>{E(l.Column)}</code></td><td><code>{E(l.Target)}</code></td><td>{E(l.Why)}</td></tr>"));
        var rules = string.Concat(DomainRules.Select(r => $"<li>{E(r)}</li>"));

        var page = template.Replace("{{MERMAID}}", E(mermaid)).Replace("{{NAV}}", nav).Replace("{{CARDS}}", cards.ToString())
            .Replace("{{FK}}", fk).Replace("{{LOGICAL}}", logical).Replace("{{RULES}}", rules)
            .Replace("{{NT}}", tables.Count.ToString()).Replace("{{NC}}", columnCount.ToString()).Replace("{{NR}}", relations.Count.ToString());
        return ("<!doctype html>\n<html lang=\"fa\" dir=\"rtl\">\n<head>\n<meta charset=\"utf-8\">\n<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n"
                + page.Replace("</style>", "body{margin:0}\n</style>\n</head>\n<body>", StringComparison.Ordinal)
                + "\n<script src=\"https://cdn.jsdelivr.net/npm/mermaid@11.4.1/dist/mermaid.min.js\"></script>\n"
                + "<script>mermaid.initialize({startOnLoad:true, theme: matchMedia(\"(prefers-color-scheme: dark)\").matches ? \"dark\" : \"default\"});</script>\n"
                + "</body>\n</html>\n").Replace("\r\n", "\n");
    }

    public static string FindRepoRoot(string start)
    {
        var dir = new DirectoryInfo(start);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "facilityservice", "FacilityService.sln"))) dir = dir.Parent;
        return dir?.FullName ?? throw new DirectoryNotFoundException("ریشهٔ ریپو (facilityservice/FacilityService.sln) پیدا نشد.");
    }
}
