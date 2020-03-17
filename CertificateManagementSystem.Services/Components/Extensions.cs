﻿namespace CertificateManagementSystem.Services.Components
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
    }
}
