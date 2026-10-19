namespace IdentityService.Domain.Models.User
{
    public class FileModel : BaseModel
    {
        #region Properties
        public int Id { get; set; }
        public string Title { get; set; }
        public int? FileTypeId { get; set; }
        public int? FileUsageTypeId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string NameWithExtension { get; set; }
        public string Extension { get; set; }
        public long? Size { get; set; }
        public string MimeType { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public int? EntityId { get; set; }
        public string EntityName { get; set; }
        public int? RegisterUserId { get; set; }
        public string MobileNumber { get; set; }
        public DateTime? RegisterDate { get; set; }
        public string DisplaySize { get; set; }
        public int? CdnNumber { get; set; }
        public string UploadFileName { get; set; }
        public string BaseFileName { get; set; }
        #endregion
    }
}
