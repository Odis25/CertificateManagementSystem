using System.IO;

namespace CertificateManagementSystem.Services.Components
{
    public static class Extensions
    {
        public static string Capitalize(this string inputString)
        {
            if (!string.IsNullOrEmpty(inputString))
            {
                var charArray = inputString.ToCharArray();
                charArray[0] = char.ToUpper(charArray[0]);

                return new string(charArray);
            }
            return inputString;
        }

        public static string ReplaceInvalidChars(this string inputString, char replaceChar)
        {
            if (!string.IsNullOrEmpty(inputString))
            {
                foreach (char badChar in Path.GetInvalidPathChars())
                {
                    inputString = inputString.Replace(badChar, replaceChar);
                }
            }           
            return inputString;
        }
    }
}
