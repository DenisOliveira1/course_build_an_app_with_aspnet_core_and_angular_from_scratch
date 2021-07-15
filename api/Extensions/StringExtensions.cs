using System.Globalization;

namespace api.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string text){
             TextInfo myTI = new CultureInfo("en-US",false).TextInfo;
             return myTI.ToTitleCase(text);
        }
    }
}