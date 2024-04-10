namespace Reddit_App.Utility
{
    public class UtilityFunction
    {
        public static  string CreateMD5(string input)
        {
            using(System.Security.Cryptography.MD5 md = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
