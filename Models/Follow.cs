using System.ComponentModel.DataAnnotations;

namespace Reddit_App.Models
{
    public class Follow
    {
        [Key]
        public int IDFollow { get; set; }
        public int FollowerID { get; set; }

        public int FollowedID { get; set; }

        public int Status { get; set; }
    }
}
