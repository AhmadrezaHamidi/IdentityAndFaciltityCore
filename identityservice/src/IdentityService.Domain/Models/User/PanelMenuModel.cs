namespace IdentityService.Domain.Models.User
{
    public class PanelMenuModel : BaseModel
    {
        public int Id { get; private set; }
        public int? ParentId { get; private set; }
        public string Title { get; private set; }
        public string EnTitle { get; private set; }
        public string Url { get; private set; }
        public string IconName { get; private set; }
        public string ActionName { get; private set; }
        public string ControllerName { get; private set; }
        public bool IsRootMenu { get; private set; }
        public bool IsInternalMenu { get; private set; }
        public DateTime CreationTime { get; private set; }
        public bool IsActive { get; private set; }

        public PanelMenuModel()
        {

        }

        public PanelMenuModel(int id, int? parentId, string title, string action, string controller,
            bool isRoot, bool isInternal, bool isActive, string url, string iconName, DateTime creationTime, string enTitle)
        {
            Id = id;
            ParentId = parentId;
            Title = title;
            Url = url;
            IconName = iconName;
            ActionName = action;
            ControllerName = controller;
            IsInternalMenu = isInternal;
            IsRootMenu = isRoot;
            IsActive = isActive;
            CreationTime = creationTime;
            EnTitle = enTitle;
        }

    }
}
