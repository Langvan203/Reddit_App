namespace Reddit_App.Request
{
    public class UpdateCommentRequest
    {
        public int PostID { get; set; }

        public string Content { get; set; }

        public IFormFile? ImageURL { get; set; }

        public int CommentID { get; set; }
    }
}
