using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace mySharedLib
{
    public static class Utility
    {

        private static string[] PRE_CLEARS = { "WEB_DL", "DD5.1", "[0-9]*(?:\\.[0-9]*)?[GM]B?" };
        private static Regex BAD_CHARS = new Regex(@"[\[.\]()_-]");
        private static string[] BAD_WORDS = { "1080P", "720P", "x26[45]", "H26[45]", "BluRay", "AC3", " DTS ", "2Audios?", "FLAME", "IMAX","BD","MP4",
            "中英","字幕", "国英","双语","音轨","香港","动作","有广告","BT下载","国粤双语中英双字","中英双字","未删节特别版","特效英语中字","中文"
        };
        private static string[] RELEASE_GROUPS = { "SPARKS", "CMCT", "FGT", "KOOK" };

        //标准化文件名
        public static string MakeValidFileName(this string name)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = $@"([{invalidChars}]*\.+$)|([{invalidChars}]+)";

            return Regex.Replace(name, invalidRegStr, "_");
        }

        //延迟执行某个操作
        //https://stackoverflow.com/questions/10458118/wait-one-second-in-running-program
        //Note Forms.Timer and Timer() have similar implementations. 
        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new Timer();;
            timer.Tick += delegate

            {
                timer.Stop();
                action.Invoke();
            };

            timer.Interval = millisecond;
            timer.Start();
        }

        //从app.config中获取设置项
        public static T GetSetting<T>(string key, T defaultValue = default(T)) where T : IConvertible
        {
            var val = ConfigurationManager.AppSettings[key] ?? "";
            T result = defaultValue;
            if (String.IsNullOrEmpty(val)) return result;
            T typeDefault = default(T);
            if (typeof(T) == typeof(string))
            {
                typeDefault = (T)(object)String.Empty;
            }
            result = (T)Convert.ChangeType(val, typeDefault.GetTypeCode());
            return result;
        }

        //清洗字符串
        public static string Purify(this string text)
        {
            var badWordsFilePath = Path.Combine(ExecutingAssemblyPath(), "BAD_WORDS.txt");
            if (File.Exists(badWordsFilePath))
            {
                var badWords = File.ReadAllLines(badWordsFilePath);
                foreach (var badWord in badWords)
                {
                    if(string.IsNullOrEmpty(badWord.Trim())) continue;
                    text =  Regex.Replace(text, badWord.Trim(), " ", RegexOptions.IgnoreCase);
                }
            }
            //var cleaned = Regex.Replace(text, "\\b" + String.Join("\\b|\\b", PRE_CLEARS) + "\\b", "", RegexOptions.IgnoreCase);
            //cleaned = BAD_CHARS.Replace(cleaned, " ");
            //cleaned = Regex.Replace(cleaned, "\\b" + String.Join("\\b|\\b", BAD_WORDS) + "\\b", "", RegexOptions.IgnoreCase);
            //cleaned = Regex.Replace(cleaned, "\\b" + String.Join("\\b|\\b", RELEASE_GROUPS) + "\\b", "", RegexOptions.IgnoreCase);
            //替换多个空格为一个空格 https://stackoverflow.com/questions/1615559/convert-a-unicode-string-to-an-escaped-ascii-string
            text = Regex.Replace(text, "[ ]{2,}"," ");
            return text.Trim();
        }

        //当前执行程序集目录
        public static string ExecutingAssemblyPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        //标准化最终的种子文件名

        public static string NormalizeTorrentFileName(this string fileName)
        {
            fileName = Regex.Replace(fileName, @"\s+", ".");//替换空白为.
            if (fileName.StartsWith("[")) return fileName;
            var regExpression = "[\u4e00-\u9fa5]";
            var i = 0;
            var hasChinese = false;
            while (i < fileName.Length)
            {
                var c = fileName.Substring(i, 1);
                if (Regex.IsMatch(c, regExpression))
                {
                    hasChinese = true;
                }
                else
                {
                    break;
                }

                i++;
            }

            return !hasChinese ? fileName : $"[{fileName.Substring(0, i)}]{fileName.Substring(i, fileName.Length - i)}";
        }

        //去除非法文件名
        public static string SanitizeFileName(this string fileName)
        {
            fileName = fileName.Replace("/", " ");
            fileName = fileName.Replace(":", "：");

            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            return invalid.Aggregate(fileName, (current, c) => current.Replace(c.ToString(), ""));
        }

        //抽取字符串中的中文
        public static string ExtractChinese(this string text)
        {
            var matches = Regex.Matches(text, "[\u4e00-\u9fa5]+");
            return String.Join(" ", matches.OfType<Match>().Where(m => m.Success));
        }

        //查找字符串的第一个年份
        public static int ExtractYear(this string text)
        {
            var match = Regex.Match(text, "(\\d{4})");
            return !match.Success ? 0 : (from Group matchGroup in match.Groups select Int32.Parse(matchGroup.Value)).FirstOrDefault(d => d != 1080 && d != 2160);
        }

        //Convert a Unicode string to an escaped ASCII string
        //https://stackoverflow.com/questions/1615559/convert-a-unicode-string-to-an-escaped-ascii-string
        public static string EncodeNonAsciiCharacters(this string value)
        {
            var sb = new StringBuilder();
            foreach (var c in value)
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
                m => ((char)Int32.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
        }
    }

    
}
