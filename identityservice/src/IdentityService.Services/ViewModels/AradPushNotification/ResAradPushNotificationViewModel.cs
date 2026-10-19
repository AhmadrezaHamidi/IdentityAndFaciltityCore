namespace IdentityService.Services.ViewModels.AradPushNotification
{
    public class ResAradPushNotificationViewModel<T> : BaseViewModel
    {
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        //در بخش لاگین که در بک اند کال می شود بصورت string است
        //در بخش ارسال ناتیفیکیشن و اضافه کردن کاربر ناتیف بصورت ابجکت است
        public T Data { get; set; }
    }
}
