using FacilityErDoc;
using FacilityService.Infrastructure;
using FacilityService.Infrastructure.DBContext;
using FacilityService.Infrastructure.Seed;
using FacilityService.Infrastructure.Services.Storage;

namespace FacilityService.Tests.Handlers;

/// <summary>
/// facilityservice/docs/Facility-ER.* از مدل EF ساخته می‌شود. اگر مدل عوض شد و سند نه، یا جدول/ستونی توضیح (HasComment) ندارد، این تست شکست می‌خورد.
/// به‌روزرسانی سند:  dotnet run --project tools/FacilityErDoc
/// </summary>
public class FacilityErDocumentTests
{
    [Fact]
    public void Entity_document_matches_ef_model()
    {
        var doc = FacilityErDocument.FromModel();
        var root = FacilityErDocument.FindRepoRoot(AppContext.BaseDirectory);
        Assert.Equal(18, doc.TableCount);
        Assert.Equal(doc.Markdown, File.ReadAllText(Path.Combine(root, FacilityErDocument.MarkdownPath)).Replace("\r\n", "\n"));
        Assert.Equal(doc.Html, File.ReadAllText(Path.Combine(root, FacilityErDocument.HtmlPath)).Replace("\r\n", "\n"));
    }

    [Fact]
    public void Every_table_and_column_has_a_persian_comment()
    {
        // FromModel برای جدول/ستون بدون توضیح یا کلید خارجی بدون معنی Exception می‌دهد
        var doc = FacilityErDocument.FromModel();
        Assert.Equal(FacilityPanelComments.Columns.Count, doc.ColumnCount);
        Assert.Equal(FacilityPanelComments.Tables.Count, doc.TableCount);
    }
}
