using System.Text.RegularExpressions;

namespace mySharedLib
{
    public static class Utility
    {


        //标准化文件名
        public static string MakeValidFileName(this string name)
        {
            var invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }
    }

    
}
