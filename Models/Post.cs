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

        public string TagID { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; }

    }
}
