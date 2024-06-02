namespace Reddit_App.Request
{
    public class CreateNewPost
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public IFormFile? Image { get; set; }
        
        public List<int> TagID { get; set; }
    }
}
