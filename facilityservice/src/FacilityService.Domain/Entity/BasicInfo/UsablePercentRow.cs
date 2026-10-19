namespace FacilityService.Domain.Entity.BasicInfo;

/// <summary>یک ردیف جدول «درصد قابل استفاده از میانگین» = یک طرح قابل انتخاب (نوع عقد × نرخ × مدت).</summary>
public sealed record UsablePercentRow(int TermMonths, ContractType ContractType, decimal Rate, decimal UsablePercent);
