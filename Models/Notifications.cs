using System.ComponentModel.DataAnnotations;

namespace Reddit_App.Models
{
    public class Notifications
    {
        [Key]
        public int NotiID { get; set; }

        public string Content { get; set; }

        public int UserID { get; set; }
    }
}
