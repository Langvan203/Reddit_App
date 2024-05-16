namespace Reddit_App.Request
{
    public class NewCommentRequest
    {
        public int PostID { get; set; }

        public string Content { get; set; }

        public IFormFile ImageURL { get; set; }
    }
}
