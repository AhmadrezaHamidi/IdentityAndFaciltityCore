# مدیریت کاربران و دسترسی — مسیرها، ورودی و خروجی

مبنا: سند «مشخصات عملکردی پنل مدیریت درخواست‌های تسهیلات سازمانی» (طرح نگین میثاق).
آدرس پایه: Admin.Api.

## مدل دسترسی در یک نگاه

```
PanelUser ──> PanelUserRole ──> Role ──> RolePanelMenu ──> PanelMenu
              (+ دامنه)                                     (کاتالوگ مجوز)
```

- **دسترسی به نقش داده می‌شود، نقش به کاربر.** هیچ جدولی دسترسی را مستقیم به کاربر نمی‌دهد.
- **دامنه روی انتساب است، نه روی نقش.** نقش «کاربر شعبه» یک بار تعریف می‌شود و هر کاربر
  همان نقش را با شعبهٔ خودش می‌گیرد. یک کاربر می‌تواند یک نقش را در چند دامنه داشته باشد.
- **`PanelMenu` هم تب است هم مجوز:**

| `Kind` | معنی | `Url` | در منو |
|---|---|---|---|
| ۱ | تب / مشاهده | دارد | دیده می‌شود |
| ۲ | عملیات (تأیید، رد، حذف) | ندارد | دیده نمی‌شود |

تب دیده می‌شود ⇔ کاربر کلید `.view` آن را دارد (معیار پذیرش ۱).

### دامنه

| `scopeType` | معنی | `scopeId` |
|---|---|---|
| `0` | سراسری | باید `null` باشد |
| `1` | شعبه | کد شعبه، الزامی |
| `2` | شرکت | شناسهٔ شرکت، الزامی |

### پاسخ‌های مشترک

| کد | معنی |
|---|---|
| `401` | نشست باطل است — توکن نامعتبر، خروج، کاربر غیرفعال، یا **تغییر دسترسی** (معیار ۶) → صفحهٔ ورود |
| `403` | این کاربر این مجوز را ندارد. دکمه‌ها را از روی کلیدهای کاربر مخفی کنید، نه با ۴۰۳ |
| `404` | پیدا نشد — یا بیرون از دامنهٔ کاربر است (معیار ۴ و ۵: فرقی نمی‌کند) |

همهٔ پاسخ‌ها در پوشش `BaseMessageResponse`:

```json
{ "isSuccess": true, "statusCode": 200, "message": "...", "data": { } }
```

---

## مسیرها

همه زیر `/UserManagement` و همه `ApiVersion = 2`.

### تب ۱ — مدیریت دسترسی‌ها

| متد | مسیر | مجوز لازم |
|---|---|---|
| `GET` | `Accesses?term=&page=&pageSize=` | `admin.permissions.read` |
| `GET` | `Accesses/{id}` | `admin.permissions.read` |
| `POST` | `Accesses` | `admin.permissions.write` |
| `PUT` | `Accesses/{id}` | `admin.permissions.write` |

### تب ۲ و ۳ — مجوزها (`Kind=0`) و گروه‌ها (`Kind=1`)

| متد | مسیر | مجوز لازم |
|---|---|---|
| `GET` | `Roles?term=&page=&pageSize=` | `admin.roles.read` |
| `GET` | `Roles/{id}` | `admin.roles.read` |
| `POST` | `Roles` | `admin.roles.write` |
| `PUT` | `Roles/{id}` | `admin.roles.write` |
| `GET` | `Groups?term=&page=&pageSize=` | `admin.groups.read` |
| `GET` | `Groups/{id}` | `admin.groups.read` |
| `POST` | `Groups` | `admin.groups.write` |
| `PUT` | `Groups/{id}` | `admin.groups.write` |

### تب ۴ — مدیریت کاربران

| متد | مسیر | مجوز لازم |
|---|---|---|
| `GET` | `PanelUsers?term=&page=&pageSize=` | `admin.users.read` |
| `GET` | `PanelUsers/{id}` | `admin.users.read` |
| `POST` | `PanelUsers` | `admin.users.write` |
| `PUT` | `PanelUsers/{id}` | `admin.users.write` |
| `PUT` | `PanelUsers/{id}/Roles` | `admin.users.assign` |
| `PUT` | `PanelUsers/{id}/Active?isActive=` | `admin.users.write` |
| `GET` | `AccessMatrix` | `admin.users.read` |
| `GET` | `SecurityAudit?action=&targetType=&page=&pageSize=` | `admin.audit.read` |

---

## ورودی و خروجی

### `POST /UserManagement/PanelUsers`

فرم سند: نام کاربری، رمز، نام کامل، کد کاربر، نام و کد شعبه + انتساب نقش با دامنه.

```json
{
  "personId": 1024,
  "userName": "b.karimi",
  "password": "...",
  "email": "b.karimi@example.com",
  "fullName": "بهنام کریمی",
  "userCode": "10457",
  "branchCode": "150002",
  "branchName": "شعبهٔ مرکزی",
  "roles": [
    { "roleId": 3, "scopeType": 1, "scopeId": "150002" }
  ]
}
```

`email` و `roles` اختیاری‌اند. `password` با همان متدی هش می‌شود که مسیر Membership
استفاده می‌کند — هیچ هش دومی در این مسیر نیست.

خروجی (`data`) برای هر سه endpoint نوشتن کاربر یکسان است:

```json
{
  "id": 77,
  "userName": "b.karimi",
  "email": "b.karimi@example.com",
  "fullName": "بهنام کریمی",
  "userCode": "10457",
  "branchCode": "150002",
  "branchName": "شعبهٔ مرکزی",
  "isActive": true,
  "permissionVersion": 1,
  "roles": [ { "roleId": 3, "scopeType": 1, "scopeId": "150002" } ]
}
```

### `PUT /UserManagement/PanelUsers/{id}`

```json
{
  "userName": "b.karimi",
  "newPassword": null,
  "email": "b.karimi@example.com",
  "fullName": "بهنام کریمی",
  "userCode": "10457",
  "branchCode": "150002",
  "branchName": "شعبهٔ مرکزی",
  "isActive": true,
  "roles": null
}
```

دو قرارداد مهم:

- `newPassword: null` یا خالی → رمز **دست نمی‌خورد**.
- `roles: null` → نقش‌ها **دست نمی‌خورند**. `roles: []` → همهٔ نقش‌ها **برداشته می‌شوند**.

### `PUT /UserManagement/PanelUsers/{id}/Roles`

بدنه، فهرست کامل است نه تغییرات جزئی — هر چه نفرستید، برداشته می‌شود.

```json
[
  { "roleId": 3, "scopeType": 1, "scopeId": "150002" },
  { "roleId": 5, "scopeType": 2, "scopeId": "9912" }
]
```

### `GET /UserManagement/SecurityAudit`

```json
{
  "items": [
    {
      "id": 4521,
      "action": "panel-user.roles.update",
      "actorPanelUserId": 1,
      "targetType": "PanelUser",
      "targetId": "77",
      "beforeJson": "[{\"RoleId\":3,\"ScopeType\":0,\"ScopeId\":null}]",
      "afterJson":  "[{\"roleId\":3,\"scopeType\":1,\"scopeId\":\"150002\"}]",
      "createdAt": "2026-10-19T08:14:22.113Z"
    }
  ],
  "total": 318,
  "page": 1,
  "pageSize": 20
}
```

`action` یکی از `panel-user.create`، `panel-user.update`، `panel-user.roles.update`،
`panel-user.active.change`، `role.update`.

### خطای اعتبارسنجی

`400` با پیام فارسی در `message`:

| پیام | علت |
|---|---|
| `نام کاربری الزامی است.` | `userName` خالی |
| `این نام کاربری قبلاً استفاده شده است.` | تکراری |
| `این کد کاربر قبلاً استفاده شده است.` | تکراری |
| `برای دامنهٔ شعبه یا شرکت، شناسهٔ دامنه الزامی است.` | `scopeType` ۱ یا ۲ بدون `scopeId` |
| `دامنهٔ سراسری شناسه نمی‌گیرد.` | `scopeType` صفر با `scopeId` |
| `رمز عبور الزامی است.` | ساخت کاربر بدون رمز |

---

## خروج اجباری با تغییر دسترسی (معیار پذیرش ۶)

> «در صورت هرگونه اعمال تغییر در سطح دسترسی کاربران، آن کاربر از سامانه لاگ‌اوت و ملزم
> به ورود با اطلاعات جدید گردد.»

هر مسیری که دسترسی را عوض می‌کند، `PanelUser.PermissionVersion` کاربران درگیر را یک
واحد بالا می‌برد و کش نشستشان را دور می‌اندازد. درخواست بعدی آن کاربر `401` می‌گیرد.

این در **رپازیتوری** انجام می‌شود، نه در Handlerها — یعنی هر Handler جدیدی هم که اضافه
شود، نمی‌تواند فراموشش کند. نقاطش:

| تغییر | کاربران bump‌شده |
|---|---|
| ویرایش مجوزهای یک نقش | همهٔ دارندگان آن نقش |
| تغییر فهرست کاربران یک نقش | هم کاربران قبلی، هم جدید |
| انتساب نقش به کاربر | همان کاربر |
| فعال/غیرفعال کردن کاربر | همان کاربر |

برای فرانت: `401` روی هر درخواستی ممکن است بیاید، نه فقط روی ورود. پاسخ درست، رفتن به
صفحهٔ ورود است، نه تلاش دوباره.

---

## آنچه این API ندارد

**دسترسی مستقیم به یک کاربر.** سند در US-A3 «برای هر کاربر ... اعطا یا سلب با چک‌باکس»
خواسته. این پیاده‌سازی دسترسی را فقط از طریق نقش می‌دهد؛ استثنا با ساختن نقش دیگر بیان
می‌شود. جدول‌های `PanelUserPermission` و `PanelUserMenu` عمداً وجود ندارند.

**Deny.** چون دسترسی مستقیم کاربر نیست، «سلب» هم نیست. نبودن یک مجوز در نقش‌های کاربر
همان معنای ندادن را دارد.
