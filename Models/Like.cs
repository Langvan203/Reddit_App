using System.ComponentModel.DataAnnotations;

namespace Reddit_App.Models
{
    public class Like
    {
        [Key]
        public int LikeID { get; set; }

        public int UserID { get; set; }

        public int PostID { get; set; }

        public bool LikeStatus { get; set; }

    }
}
