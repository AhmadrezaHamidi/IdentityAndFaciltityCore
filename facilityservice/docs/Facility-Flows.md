## راهنمای Flowها — پنل تسهیلات سازمانی (نگین میثاق)

**ورود و منو از Admin.Api است** (`POST /Authentication/Login`، `GET /Authentication/MainMenu`، `GET /Authentication/Me`).
همان کوکی ورود برای این سرویس هم کار می‌کند؛ Facility آن را از Identity (`POST /Internal/Authorization/Resolve`) چک می‌کند.

**آدرس پایه:** Facility = `http://localhost:5089` ، Identity (Admin.Api) = `http://localhost:5079`. همهٔ مسیرهای این سند **کامل و نسبت به آدرس پایه** نوشته شده‌اند؛ مسیرهای `/Authentication/...` و `/UserManagement/...` مال Identity هستند، بقیه مال Facility.

**قواعد عمومی**
- **401** ← نشست باطل است (خروج، غیرفعال شدن یا تغییر دسترسی) → صفحهٔ ورود.
- **403** ← این کاربر این مجوز را ندارد. دکمه‌ها را از روی کلیدهای `Me` مخفی کن، نه با 403.
- **دامنه**: کاربر شعبه فقط شرکت‌های شعبهٔ خودش، کاربر شرکت فقط شرکت خودش، ادمین و دیجیتال همه.
  شرکتی که در دامنه نیست مثل «پیدا نشد» رفتار می‌کند (حتی با دستکاری URL).
- ویرایش‌ها `rowVersion` دریافتی را برمی‌گردانند. فایل‌ها با `multipart/form-data`.
- تاریخ‌ها `yyyy-MM-dd` میلادی؛ ماه‌ها `«۱۴۰۵/۰۷»` شمسی. مبلغ‌ها ریال.

---

### G1 — اطلاعات پایه طرح (ادمین)
1. `GET /BasicInfo/Schemes` ← انتخاب طرح کلان (نگین میثاق، ...)
2. `GET /BasicInfo/{schemeId}` ← فرم
3. `PUT /BasicInfo/{schemeId}` ← ذخیره (با `rowVersion`)
4. دلایل: `GET /BasicInfo/Reasons?type=1..6` ، `POST /BasicInfo/Reasons` ، `PUT|DELETE /BasicInfo/Reasons/{id}`

### G2 — ثبت شرکت (کاربر شعبه / ادمین)
1. `GET /BasicInfo/Schemes` و `GET /BasicInfo/{schemeId}/Plans` ← چک‌باکس طرح‌های تفاهم‌نامه
2. `POST /Companies` (فرم + `agreementFile`)
3. `GET /Companies?term=` ← فهرست با «X فعال از Y» و ثبت‌کننده

### G3 — میانگین موجودی و مستندات
1. `GET /Companies/Lookup` ← انتخاب شرکت
2. `GET /Companies/{id}/Balances` ← ماه‌ها + مانده اعتبار + `hasDocuments` (هینت «مستندی پیوست نشده»)
3. `PUT /Companies/{id}/Balances` (بعد از تأیید کاربر)
4. `POST /Companies/{id}/Balances/Documents` (چند فایل + نوع مستند)، `GET /Companies/{id}/Balances/Documents`، `GET /Companies/{id}/Balances/Documents/{docId}`، `DELETE /Companies/{id}/Balances/Documents/{docId}` (ادمین)

### G4 — لغو قرارداد
1. `GET /BasicInfo/Reasons?type=1`
2. `POST /Companies/{id}/Cancel` (تاریخ، دلیل، توضیح، `letterFile` همه الزامی)

### G5 — معرفی پرسنل (منابع انسانی / مدیر شرکت)
1. `GET /Companies/Lookup` (برای نقش شرکت فقط شرکت خودش)
2. دستی: `POST /Companies/{companyId}/Personnel` ← وضعیت «در انتظار تأیید مدیر شرکت»
3. گروهی: `GET /Personnel/Import/Template` → `POST /Companies/{companyId}/Personnel/Import` (`file`) → خطای هر ردیف
4. ردشده: `GET /Personnel/{id}` (دلیل و تاریخچه) → `PUT /Personnel/{id}` (اصلاح و ارسال دوباره)
5. حذف: `POST /Personnel/{id}/Delete` (توضیح + `file`)

### G6 — تأیید افراد معرفی‌شده (مدیر شرکت)
1. `GET /Companies/{companyId}/Personnel?status=1`
2. `POST /Personnel/Decisions` ← `{ personnelIds, decision: 1|2, reasonId, description }`

### G7 — ثبت درخواست وام (منابع انسانی / مدیر شرکت)
1. `GET /Companies/{companyId}/Credit` ← بنر وضعیت + اعتبار هر طرح
2. `GET /Companies/{companyId}/Personnel?onlyAvailableForLoan=true` ← پرسنل تأییدشده به ترتیب اولویت
3. هنگام تایپ: `POST /Companies/{companyId}/LoanRequests/Preview` ← هینت‌ها (مجموع، مجاز/غیرمجاز، «حداکثر N نفر»)
4. `POST /Companies/{companyId}/LoanRequests` ← `{ planId, baseDate, mode: 1|2, amount | items[] }` ← صف مدیر شرکت
5. `GET /LoanRequests?companyId=` ← «درخواست‌های وام این شرکت»
6. اصلاح قبل از تأیید: `POST /LoanBatches/{batchId}/Members` ، `DELETE /LoanBatches/{batchId}/Members/{requestId}` ، `POST /LoanRequests/{id}/Cancel`

### G8 — تأیید مدیر شرکت
1. `GET /ManagerApprovals?companyId=`
2. `POST /ManagerApprovals/Decisions` ← تأیید: صف «تأیید اعتبارات»؛ رد: دلیل و توضیح الزامی

### G9 — تأیید اعتبارات (کاربر شعبه / ادمین)
1. `GET /Approvals/PendingCounts` ← عدد روی تب‌ها
2. `GET /CreditApprovals?companyId=`
3. `POST /CreditApprovals/Decisions` (فرم: `requestIds`، `decision`، `reasonId`، `description`، `attachment`) — همه در یک تراکنش
4. بعد از تأیید: `PUT /CreditApprovals/{requestId}/PostApprovalStage` (برای «عدم اعطای غیرحضوری» دلیل `type=3`)
5. اگر «تأیید شعبهٔ دیجیتال» در اطلاعات پایه فعال است: `GET /DigitalApprovals` → `POST /DigitalApprovals/Decisions`

### G10 — داشبورد
1. `GET /Dashboard?from=&to=` ← KPIها و نمودارها
2. `GET /Dashboard/CompanyRanking?month=&sortBy=`
3. `GET /Dashboard/Exports/{Companies|Personnel|LoanRequests|MonthlyAverages|RemovedCompanies}` ← CSV

### G11 — گزارش تغییرات
1. شرکت/پرسنل/وام: `GET /ChangeLogs?category=1..3` ، خروجی `GET /ChangeLogs/Export`
2. کاربران: `GET /UserManagement/SecurityAudit` در **Admin.Api**
3. بازگردانی (فقط ادمین): `POST /ChangeLogs/{id}/Restore`
