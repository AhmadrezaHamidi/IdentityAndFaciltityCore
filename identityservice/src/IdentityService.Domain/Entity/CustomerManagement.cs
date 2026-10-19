using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entity
{

    /// <summary>
    /// وضعیت دسترسیِ یک کاربر نهایی (مشتری) به یک خدمت — تب «تغییر وضعیت دسترسی به
    /// هر خدمت» در فلوچارت Figma «مدیریت مشتریان». جدول جدید و ضروری است: هیچ
    /// معادلی در دامنهٔ فعلی وجود نداشت (PolicyEntity محدودیت نرخ است، نه سوییچ
    /// فعال/غیرفعال به‌ازای کاربر و خدمت).
    /// </summary>
    public sealed class UserServiceAccessEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        /// <summary>کلید خدمت (مثلاً Kyc، MobileChange، AccountSettings، CardOperations، ...).</summary>
        public string ServiceKey { get; set; } = null!;

        public bool IsEnabled { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public int? UpdatedByPanelUserId { get; set; }

        public  UserEntity User { get; set; } = null!;
    }

}
