using System;
using System.Data.Common;
using System.Data.SQLite;

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
    }
}
