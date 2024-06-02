namespace Reddit_App.Dto
{
    public class GetPostDto
    {
       public int PostID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Image { get; set; }
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Avata { get; set; }

        public List<TagDto> ListTag { get; set; }

        public int TotalComment { get; set; }

        public int TotalLike { get; set; }

        public List<GetListCommentPostDto> ListComment { get; set; }

        public List<GetLikeInPostDto> ListLike { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
