using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace MovieTorrents.Common
{
    public static class Common
    {
        public static string GetReaderFieldString(this SQLiteDataReader reader, string fieldName)
        {
            return Convert.IsDBNull(reader[fieldName]) ? string.Empty : (string)reader[fieldName];
        }

        public static string GetReaderFieldString(this DbDataReader reader, string fieldName)
        {
            return Convert.IsDBNull(reader[fieldName]) ? string.Empty : ((string)reader[fieldName]).Trim();
        }

        public static string GetAllText(this Match match, int groupIndex = 1)
        {
            if (!match.Success) return null;
            var list = new List<string>();
            while (match.Success)
            {
                list.Add(match.Groups[groupIndex].Value);
                match = match.NextMatch();
            }

            return string.Join(",", list);
        }

        const long RefDateInt = 13125120818;
        private const string RefDateString = "2016-12-02 10:53:38";
        public static string SqlLiteToDateTime(this long? n)
        {
            if (n == null) return "";
            var refDate = DateTime.Parse(RefDateString);
            return refDate.AddSeconds(n.Value / 10000000 - RefDateInt).ToString("yyyy-MM-dd HH:mm");
        }

        public static long ToSqlLiteInt64(this DateTime dateTime)
        {
            var refDate = DateTime.Parse(RefDateString);
            return ((long)(dateTime - refDate).TotalSeconds + RefDateInt) * 10000000;

        }
    }
}
