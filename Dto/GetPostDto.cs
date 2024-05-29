namespace Reddit_App.Dto
{
    public class GetPostDto
    {
       public int PostID { get; set; }

       public int UserID { get; set; }

       public string UserName { get; set; }

        public string Avata { get; set; }

        public string TagName { get; set; }

        public int TotalComment { get; set; }
    }
}
