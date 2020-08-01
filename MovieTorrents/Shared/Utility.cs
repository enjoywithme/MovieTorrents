using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTorrents.Shared
{
    public static class Utility
    {
        public static string GetReaderFieldString(this SQLiteDataReader reader, string fieldName)
        {
            return Convert.IsDBNull(reader[fieldName]) ? string.Empty : (string)reader[fieldName];
        }
    }
}
