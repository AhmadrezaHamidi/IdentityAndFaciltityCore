# FacilityService — پنل تسهیلات (طرح نگین میثاق)

Solution جدا، با همان ساختار IdentityService:

```
facilityservice/
├── FacilityService.sln
├── src/
│   ├── FacilityService.Domain          Entity، Enums، Rules، Helpers، Exceptions
│   ├── FacilityService.Contracts       ReadModels، CommandResponses، Persistence (IFacilityPanelDbContext)
│   ├── FacilityService.Application     Handler/<بخش>/{Commands,Queries,Shared}، Authorization
│   ├── FacilityService.Infrastructure  DBContext، Configuration، Migrations، Seed، Services/Storage (Minio)
│   ├── FacilityService.IoC             DependencyContainer
│   └── FacilityService.Api             Controllers/V2، Attributes، Swagger، FacilityPanelErrorHandling
├── test/FacilityService.Tests          سناریوهای کسب‌وکار (Handler) + HTTP از کنترلر تا دیتابیس
├── tools/FacilityErDoc                 تولید سند ER
├── Database/                           اسکریپت Migration برای SQL Server
└── docs/                               ER و Flowها
```

## اجرا و تست
```powershell
cd facilityservice
dotnet build FacilityService.sln
dotnet test  FacilityService.sln                                   # SQLite درون‌حافظه؛ SQL Server/Minio لازم نیست
dotnet run --project src\FacilityService.Api                       # http://localhost:55454/swagger
```
در حالت Development، `Identity:BaseUrl` روی `http://localhost:5005` (Admin.Api) است و کلید `InternalApi:Key` باید با Admin.Api یکی باشد.

## دادهٔ تست (SQL Server)
1. `Database/facility_panel_migrations.sql` روی دیتابیس Facility (جداول + طرح نگین میثاق)
2. `Database/02_test_data.sql` روی همان دیتابیس: ۳ شرکت، ۱۴ پرسنل، ۸ درخواست وام در همهٔ وضعیت‌ها، گزارش تغییرات.
   شناسهٔ شرکت‌ها را در خروجی چاپ می‌کند.
3. `../identityservice/Database/08_test_users.sql` روی دیتابیس Identity: هفت کاربر `test.*` برای ۵ نقش با دامنهٔ درست،
   و بررسی اینکه ماتریس مجوز نقش‌ها دقیقاً همان کد است (خروجی خالی = درست).
4. (اختیاری، برای پر شدن همهٔ تب‌ها) `Database/03_more_test_data.sql` روی Facility: ۷ شرکت دیگر در ۴ شعبه
   (فعال، روز انتقال نیست، قرارداد تازه، لغوشده، منقضی، شعبهٔ دیجیتال)، ۴۴ پرسنل، ۳۳ درخواست وام در همهٔ ۸ وضعیت و ۴ مرحلهٔ پس از تأیید،
   بارگذاری گروهی و حدود ۲۰۰ ردیف گزارش تغییرات؛ بعد `../identityservice/Database/08b_more_test_data.sql` روی Identity:
   ۱۱ کاربر دیگر (مدیر و منابع انسانی دلتا و زتا، منشی، ناظر ستاد، کاربر غیرفعال و …)، ۲ نقش و ۲ گروه، دسترسی جدا و لاگ امنیتی.
   هر دو چندباره‌اجراشدنی‌اند و بخش پاک‌سازی دارند.
