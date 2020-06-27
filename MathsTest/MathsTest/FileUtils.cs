namespace MathsTest
{
    public class FileUtils
    {
        public static string GetUserFileName(string userName)
        {
            return $"{userName}.gitignore";
        }
    }
}