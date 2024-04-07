using System.ComponentModel.DataAnnotations;

namespace Reddit_App.Models
{
    public class Share
    {
        [Key]
        public int ShareID { get; set; }

        public int UserID { get; set; }

        public int PostID { get; set; }

    }
}
