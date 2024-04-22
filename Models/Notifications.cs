using System.ComponentModel.DataAnnotations;

namespace Reddit_App.Models
{
    public class Notifications
    {
        [Key]
        public int NotiID { get; set; }

        public string Content { get; set; }

        public int UserID { get; set; }

        // cần thêm field TimeCreated để sinh hiển thị thời gian đã nhận thông báo
    }
}
