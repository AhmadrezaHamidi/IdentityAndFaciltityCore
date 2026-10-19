using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

/// <summary>۵ دکمهٔ خروجی CSV (UTF-8 با BOM برای Excel)؛ فقط دادهٔ دامنهٔ کاربر.</summary>
public enum DashboardExport : byte { Companies = 1, Personnel = 2, LoanRequests = 3, MonthlyAverages = 4, RemovedCompanies = 5 }
