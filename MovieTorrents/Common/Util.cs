using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieTorrents.Common
{
    public static class Util
    {
        public static string GetReaderFieldString(this SQLiteDataReader reader, string fieldName)
        {
            return Convert.IsDBNull(reader[fieldName]) ? string.Empty : (string)reader[fieldName];
        }

        public static string GetReaderFieldString(this DbDataReader reader, string fieldName)
        {
            return Convert.IsDBNull(reader[fieldName]) ? string.Empty : (string)reader[fieldName];
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
    }
}
