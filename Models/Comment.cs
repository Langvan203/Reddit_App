using System.ComponentModel.DataAnnotations;

namespace Reddit_App.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        public int PostID { get; set; }

        public string Content { get; set; }

        public int UserID { get; set; }

        public int CommentStatus { get; set; }
    }
}
