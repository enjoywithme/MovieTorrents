using System;
using System.Configuration;
using System.Globalization;
using System.Text;
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

        //延迟执行某个操作
        //https://stackoverflow.com/questions/10458118/wait-one-second-in-running-program
        //Note Forms.Timer and Timer() have similar implementations. 
        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new System.Windows.Forms.Timer();;
            timer.Tick += delegate

            {
                timer.Stop();
                action.Invoke();
            };

            timer.Interval = millisecond;
            timer.Start();
        }

        public static T GetSetting<T>(string key, T defaultValue = default(T)) where T : IConvertible
        {
            var val = ConfigurationManager.AppSettings[key] ?? "";
            T result = defaultValue;
            if (string.IsNullOrEmpty(val)) return result;
            T typeDefault = default(T);
            if (typeof(T) == typeof(string))
            {
                typeDefault = (T)(object)string.Empty;
            }
            result = (T)Convert.ChangeType(val, typeDefault.GetTypeCode());
            return result;
        }

        //Convert a Unicode string to an escaped ASCII string
        //https://stackoverflow.com/questions/1615559/convert-a-unicode-string-to-an-escaped-ascii-string
        public static string EncodeNonAsciiCharacters(this string value)
        {
            var sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    var encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string DecodeEncodedNonAsciiCharacters(this string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
        }
    }

    
}
