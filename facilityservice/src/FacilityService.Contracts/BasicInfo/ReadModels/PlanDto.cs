using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.BasicInfo.ReadModels;

public sealed record PlanDto(int Id, ContractType ContractType, decimal Rate, int TermMonths, decimal UsablePercent, string Title);

/// <summary>
/// طرح‌های قابل انتخاب، همراه با سقف تعداد طرح هر شرکت.
///
/// چرا پوشش گرفت: فرانت برای محدود کردن انتخاب طرح در فرم شرکت به
/// <paramref name="MaxPlansPerCompany"/> نیاز دارد و تا امروز فقط در «اطلاعات پایهٔ طرح» بود —
/// یعنی فرانت باید یک سرویس دیگر را هم صدا می‌زد که مجوز ویرایش اطلاعات پایه می‌خواهد
/// و کاربر شعبه آن را ندارد.
/// </summary>
public sealed record PlanListDto(int MaxPlansPerCompany, IReadOnlyList<PlanDto> Plans);

// ── فهرست‌های داینامیک (دلایل لغو، رد، عدم اعطای غیرحضوری، نوع مستند) ──
