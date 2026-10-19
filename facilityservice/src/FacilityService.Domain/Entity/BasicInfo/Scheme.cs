namespace FacilityService.Domain.Entity.BasicInfo;

/// <summary>طرح کلان (نگین میثاق، شجره طیبه، هم‌پیمان، ...) — فیلد ۲۰۶.</summary>
public class Scheme
{
    public int Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private Scheme() { }

    public Scheme(string code, string title)
    {
        Code = code;
        Title = title;
    }
}
