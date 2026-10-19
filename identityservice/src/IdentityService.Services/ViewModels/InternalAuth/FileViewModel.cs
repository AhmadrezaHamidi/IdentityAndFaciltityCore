namespace IdentityService.Services.ViewModels.InternalAuth
{
    public class FileViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? FileTypeId { get; set; }
        public int? FileUsageTypeId { get; set; }
        public int? CdnNumber { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }
        public string FullUrl { get; set; }
        public string Name { get; set; }
        public string NameWithExtension { get; set; }
        public string Extension { get; set; }
        public long? Size { get; set; }
        public string MimeType { get; set; }
        public string DisplaySize { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public int? EntityId { get; set; }
        public string EntityName { get; set; }
        public int? RegisterUserId { get; set; }
        public DateTime? RegisterDate { get; set; }
    }
}
