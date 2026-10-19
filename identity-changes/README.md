# تغییرات Identity نسبت به نسخهٔ خام

مبنا: شاخهٔ `identityservice-base` ریپوی adminPanel — همان چیزی که ۱۷ مهر ۱۴۰۵ تحویل داده شد.

این پوشه فقط برای مرور است؛ کد قابل build در `../identityservice/` است و این‌جا کپی همان
فایل‌هاست تا دیدن تغییرات نیاز به git نداشته باشد.

```
identity-changes/
├── added/      ۱۲ فایل تازه، با همان مسیر اصلی‌شان
├── modified/   ۹ فایل عوض‌شده، نسخهٔ نهایی
└── diff/       identity.patch — دیف کامل، برای دیدن سطر‌به‌سطر چه عوض شده
```

**هیچ فایلی حذف نشده.** `PanelUserMenu` و `PanelUserPermission` در نسخهٔ خام وجود نداشتند،
پس چیزی برای برداشتن نبود؛ مدل دسترسی روی همان زنجیرهٔ موجود ساخته شد:

```
PanelUser → PanelUserRole → Role → RolePanelMenu → PanelMenu
            (دامنه این‌جاست)              (کلید مجوز این‌جاست)
```

جمع: ۲۱ فایل، ۲۴۴۷ سطر افزوده، ۵۷۱ سطر برداشته.

---

## فایل‌های تازه (۱۲)

### مدل مجوز — `src/IdentityService.Domain/Authorization/`

| فایل | چیست |
|---|---|
| `AccessScope.cs` | دامنهٔ دسترسی: سراسری / شعبه / شرکت، و `Covers(ResourceScope)` که می‌گوید این دامنه فلان منبع را می‌پوشاند یا نه. تنها جایی که قاعدهٔ «کاربر شعبه فقط شعبهٔ خودش» نوشته شده. |
| `AdminPermissionKeys.cs` | ۱۲ کلید `admin.*` پنل مدیریت کاربران. دقیقاً همان کلیدهای v2، تا هر دو سمت یک زبان داشته باشند. |
| `EffectivePermissions.cs` | مجوزهای مؤثر یک کاربر: از `PanelUserRoles` می‌رود روی `Role.RolePanelMenus` و `PanelMenu.PermissionKey` را با دامنهٔ **انتسابِ نقش** جمع می‌زند. قلب «دسترسی به نقش، نقش به کاربر، با دامنه». |
| `PanelSession.cs` | رکورد `PanelSession` و `IPanelSessionResolver`. در Domain گذاشته شد تا Infrastructure بتواند پیاده‌اش کند و Api فقط به اینترفیس وابسته باشد. |

### موجودیت‌ها — `src/IdentityService.Domain/Entity/`

| فایل | چیست |
|---|---|
| `PanelAuthorizationEntities.Partial.cs` | `partial` روی موجودیت‌های اسکفولدشده، تا فایل‌های تولیدی دست نخورند: `PanelUser` (`FullName`, `UserCode`, `BranchCode`, `BranchName`, `PermissionVersion`, `LastPermissionChangeAt`)، `Role` (`AllowedScopeType`, `IsSystem`)، `PanelUserRole` (`ScopeType`, `ScopeId`, `CreatedAt`, `CreatedByPanelUserId`, `Scope`)، `PanelMenu` (`PermissionKey`, `Kind`, `SortOrder`). |
| `PanelSecurityAuditLogEntity.cs` | لاگ امنیتی پنل: چه کسی، چه وقت، روی چه کسی، قبل و بعد. دستهٔ «کاربران» در گزارش تغییرات از همین خوانده می‌شود. |

### اجرای مجوز — `Admin.Api/Filters/`

| فایل | چیست |
|---|---|
| `RequirePanelPermissionAttribute.cs` | یک attribute بدون منطق + یک `IAsyncAuthorizationFilter` سراسری. نزدیک‌ترین اعلان برنده است (اکشن بر کنترلر)، و مسیری که هیچ اعلانی ندارد **بسته و لاگ می‌شود** — fail-closed. |

### پیاده‌سازی — `src/IdentityService.Infrastructure/`

| فایل | چیست |
|---|---|
| `Repositories/UserManagementPanel/PanelSessionResolver.cs` | کوکی یا Bearer ← `IJwtService.ValidateToken` ← نشست در Redis ← کاربر فعال ← `EffectivePermissions.Build`. کش ۳۰ ثانیه‌ای در حافظه (عمداً نه Redis: با `PermissionVersion` باطل می‌شود و پراکندگی‌اش ارزان‌تر از یک کش اشتراکی است). |

### کاربردها — `src/IdentityService.Application/`

| فایل | چیست |
|---|---|
| `Handler/Admin/UserManagementPanel/PanelUserWriteHandlers.cs` | ساخت، ویرایش و انتساب نقش کاربر پنل. رمز فقط از یک مسیر هش می‌شود (`UserModel().SetPassword`) و دامنهٔ هر انتساب با `Role.AllowedScopeType` اعتبارسنجی می‌شود. |

### دیتابیس و سند

| فایل | چیست |
|---|---|
| `Database/01_panel_authorization.sql` | ستون‌های لازم روی جدول‌های موجود + جدول لاگ امنیتی + ۵ نقش سند. چندبار قابل اجراست. |
| `Database/02_seed_permissions_and_matrix.sql` | ۴۷ کلید مجوز، ۹ تب، و ماتریس پیش‌فرض ۵ نقش طبق سند. دو تناقض سند با ⚠ Q1/Q2 علامت خورده‌اند. |
| `docs/Panel-Access-API.md` | ۲۰ مسیر با ورودی/خروجی، جدول دامنه، جدول خطاهای ۴۰۰، و دو قرارداد: `newPassword: null` رمز را دست نمی‌زند؛ `roles: null` انتساب‌ها را دست نمی‌زند ولی `roles: []` پاکشان می‌کند. |

---

## فایل‌های عوض‌شده (۹)

| فایل | چه عوض شد و چرا |
|---|---|
| `Admin.Api/Controllers/V2/UserManagement/UserManagementController.cs` | هر ۱۶ مسیر کلید مجوز اعلام می‌کند. در نسخهٔ خام ۱۵ مسیر فقط `[AuthenticationUserAdminByCookie]` داشتند و تنها محافظشان پنهان بودن منو در سایدبار بود — چیزی که سند صریحاً رد می‌کند («حتی با دست‌کاری URL»). |
| `Admin.Api/Program.cs` | ثبت فیلتر سراسری مجوز. بدونش attributeها فقط علامت‌اند. |
| `src/IdentityService.Application/Handler/Admin/MainMenuHandler.cs` | ردیف‌های `Kind=2` (عملیات) از سایدبار بیرون می‌مانند: `menus.Where(m => !string.IsNullOrWhiteSpace(m.Url))`. بدون این، «تأیید اعتبارات» و «رد اعتبارات» به‌عنوان آیتم منو ظاهر می‌شدند. |
| `.../UserManagementPanel/PanelUserHandlers.cs` | خواندن کاربر با نقش‌ها و دامنهٔ هر انتساب؛ `AsSplitQuery` برای Includeهای سه‌سطحی. |
| `.../UserManagementPanel/RoleGroupHandlers.cs` | نقش با کلیدهای مجوزش و دامنهٔ مجازش خوانده و نوشته می‌شود. |
| `src/IdentityService.Domain/Services/UserManagementPanel/IPanelAccessControlRepository.cs` | امضاها برای دامنه و نسخهٔ مجوز. |
| `src/IdentityService.Infrastructure/DBContext/ApplicationDBContext.cs` | `DbSet` لاگ امنیتی. |
| `src/IdentityService.Infrastructure/DependencyInjectionExtension.cs` | ثبت `IPanelSessionResolver`. |
| `.../Repositories/UserManagementPanel/PanelAccessControlRepository.cs` | بزرگ‌ترین تغییر (۶۵۳ سطر). دو چیزی که باید دیده شود: **(۱)** ویرایش یک نقش دیگر دامنهٔ دارندگانش را پاک نمی‌کند — در نسخهٔ قبلی `ReplaceRoleLinksAsync` دامنهٔ همه را صفر می‌کرد و یک کاربر شعبه بی‌صدا سراسری می‌شد؛ حالا دامنهٔ قبلی هر کاربر نگه داشته می‌شود. **(۲)** `BumpPermissionVersionAsync` در همان نقطهٔ گلوگاه صدا می‌شود، روی کاربرهای قبلی و جدید با هم، تا نشست کسی که دسترسی‌اش برداشته شد باطل شود. |

---

## چیزی که هنوز نشده

Identity با `dotnet build` بررسی نشده — در محیطی که این کد نوشته شد SDK نبود. دو فرضی که
باید در اولین build تأیید شود:

- شکل tuple برگشتی `IJwtService.ValidateToken`
- رفتار `UserModel.SetPassword` (فرض: خودش salt و hash را می‌گذارد)

بقیهٔ امضاها از خود نسخهٔ خام خوانده شده‌اند.
