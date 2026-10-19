namespace IdentityService.Domain.Models.User
{
    public class UserMessageModel : BaseModel
    {
        public int MessageId { get; private set; }
        public bool Read { get; private set; }
        public string AuthorName { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public long DateTime { get; private set; }



        public UserMessageModel()
        {

        }

        public UserMessageModel(int messageId, bool read)
        {
            MessageId = messageId;
            Read = read;
        }

        public void SetAuthorName(string name)
        {
            AuthorName = name;
        }

        public void SetMessage(string title, string body, DateTime sentTime)
        {
            Title = title;
            Body = body;
            DateTime = sentTime.ToTimeStampSeconds();
        }
    }
}
