using System.ComponentModel.DataAnnotations;

namespace Reddit_App.Models
{
    public class users
    {
        [Key]
        public int UserID { get; set; }

        public string UserName { get; set; }    

        public string PassWord { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Role { get; set; }

        public bool Status { get; set; }
    }
}
