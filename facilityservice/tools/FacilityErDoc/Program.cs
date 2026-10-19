using FacilityErDoc;

// اجرا از ریشهٔ ریپو:  dotnet run --project tools/FacilityErDoc
// خروجی: facilityservice/docs/Facility-ER.md (نمودار mermaid برای GitHub) و facilityservice/docs/Facility-ER.html (صفحهٔ مستقل)
var root = args.Length > 0 ? args[0] : FacilityErDocument.FindRepoRoot(AppContext.BaseDirectory);
var doc = FacilityErDocument.FromModel();
File.WriteAllText(Path.Combine(root, FacilityErDocument.MarkdownPath), doc.Markdown);
File.WriteAllText(Path.Combine(root, FacilityErDocument.HtmlPath), doc.Html);
Console.WriteLine($"{doc.TableCount} جدول، {doc.ColumnCount} فیلد، {doc.ForeignKeyCount} کلید خارجی ← {FacilityErDocument.MarkdownPath}، {FacilityErDocument.HtmlPath}");
