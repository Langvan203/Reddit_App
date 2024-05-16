namespace Reddit_App.Request
{
    public class UserRegisterRequest
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public IFormFile ImageAvata { get; set; }
    }
}
