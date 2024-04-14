namespace Reddit_App.Request
{
    public class UpdateUserInfoRequest
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

    }
}
