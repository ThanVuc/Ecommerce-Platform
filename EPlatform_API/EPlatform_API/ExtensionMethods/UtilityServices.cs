using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.ExtensionMethods
{
    public static class UtilityServices
    {
        public static DateTime ConvertUTCToVietNam(DateTime utcNow)
        {
            var vietNamOffSet = TimeSpan.FromHours(7);
            var vietnamTime = utcNow + vietNamOffSet;
            return vietnamTime;
        }


        public static string GenerateSlug(string phrase)
        {
            // Only limited the duplicate slug, not 100% unique
            string str = phrase.ToLower();
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "-"); // replace spaces
            string uniqueIdentifier = GenerateRandomString(5);
            str = $"{str}-{uniqueIdentifier}--{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"; // append the unique identifier to the slug
            return str;
        }

        public static string GenerateRandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int charsLength = chars.Length;
            char[] randomString = new char[length];
            var ramdom = new Random();
            for(int i=0; i<length; i++)
            {
                randomString[i] = chars[ramdom.Next(charsLength)];
            }
            return new string(randomString);
        }
    
        public static string ConvertBigNumberToShortNumber(long number)
        {
            if(number < 1000)
            {
                return number.ToString();
            }
            if(number < (double)1000000)
            {
                return $"{(double)number/1000}K";
            }
            if(number < 1000000000)
            {
                return $"{(double)number/1000000}M";
            }
            return $"{(double)number/1000000000}B";
        }
    }
}