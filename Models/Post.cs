using System.ComponentModel.DataAnnotations;

namespace Reddit_App.Models
{
    public class Post
    {
        [Key]
        public int PostID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Image { get; set; }

        public int UserID { get; set; }

        public int TagID { get; set; }
    }
}
