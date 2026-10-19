namespace FacilityService.Domain.Entity.BasicInfo;

public class SchemePlan
{
    public int Id { get; private set; }
    public int SchemeId { get; private set; }
    public ContractType ContractType { get; private set; }
    public decimal Rate { get; private set; }
    public int TermMonths { get; private set; }
    public decimal UsablePercent { get; private set; }
    public bool IsActive { get; private set; } = true;

    private SchemePlan() { }

    public SchemePlan(int schemeId, ContractType contractType, decimal rate, int termMonths, decimal usablePercent)
    {
        SchemeId = schemeId;
        ContractType = contractType;
        Rate = rate;
        TermMonths = termMonths;
        UsablePercent = usablePercent;
    }

    public bool Matches(ContractType type, decimal rate, int term) => ContractType == type && Rate == rate && TermMonths == term;

    public void Activate(decimal usablePercent)
    {
        UsablePercent = usablePercent;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;

    /// <summary>فقط طرح قرض‌الحسنه، وقتی نرخ ثابت اطلاعات پایه عوض می‌شود.</summary>
    internal void ChangeRate(decimal rate) => Rate = rate;

    public string ContractTitle => ContractType == ContractType.QardHasan ? "قرض‌الحسنه" : "مرابحه";

    public string Title => $"{ContractTitle} {PersianText.Digits(Rate.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture))}٪ — {PersianText.Number(TermMonths)} ماهه";
}
