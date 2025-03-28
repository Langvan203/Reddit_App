﻿using System.ComponentModel.DataAnnotations;

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

        public string Image { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int CommentParentID { get; set; }
    }
}
