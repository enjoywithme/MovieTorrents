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
            return Convert.IsDBNull(reader[fieldName]) ? string.Empty : (string)reader[fieldName];
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
    }
}
