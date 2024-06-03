using System.ComponentModel.DataAnnotations;

namespace Reddit_App.Models
{
    public class Tags
    {
        [Key]
        public int TagID { get; set; }

        public string TagName { get; set; }

        public int TagStatus { get; set; }
    }
}
