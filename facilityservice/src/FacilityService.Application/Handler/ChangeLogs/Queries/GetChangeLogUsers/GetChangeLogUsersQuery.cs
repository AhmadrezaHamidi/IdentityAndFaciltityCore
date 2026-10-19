using FacilityService.Application.Common;

namespace FacilityService.Application.Handler.ChangeLogs.Queries.GetChangeLogUsers;

/// <summary>
/// کاربرانی که در دامنهٔ کاربر جاری ردیفی در گزارش تغییرات دارند، برای پر کردن سلکت فیلتر «کاربر».
///
/// چرا از خود لاگ خوانده می‌شود و نه از Identity: فیلتر باید فقط کسانی را نشان دهد که این
/// کاربر ردیف‌هایشان را می‌بیند؛ فهرست کامل کاربران پنل هم دامنه را لو می‌دهد و هم پر از
/// کسانی است که انتخابشان صفر نتیجه می‌دهد.
/// </summary>
public sealed record GetChangeLogUsersQuery : FacilityBaseQuery<FacilityBaseMessageResponse<IReadOnlyList<RegistrarDto>>>;
