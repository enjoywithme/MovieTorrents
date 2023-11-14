using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;

namespace MovieTorrents.Common;

public class TorrentFilter
{
    public int RecordsLimit { get; set; } = 100;
    public bool Recent { get; set; }
    public bool HideSameSubject { get; set; } = true;
    public bool NotWatched { get; set; }
    public bool Watched { get; set; }
    public bool NotWant { get; set; }
    public bool SeeLater { get; set; }
    public bool HasDoubanId { get; set; }
    public bool NoDoubanId { get; set; }

    public bool? OrderRatingDesc { get; set; } = true;
    public bool? OrderYearDesc { get; set; }
    private readonly List<string> _filterFields = new() { "rating", "year" };

    //构造搜索SQL
    private bool ProcessFilterFields(SQLiteCommand command, StringBuilder sb, string text)
    {
        if (!text.Contains(":")) return false;
        var splits = text.Split(':');
        if (splits.Length != 2) return false;
        var fldName = splits[0].ToLower();
        if (!_filterFields.Contains(fldName)) return false;
        var greaterLess = string.Empty;
        var fldValue = splits[1];
        if (fldValue.StartsWith(">") || fldValue.StartsWith("<"))
        {
            greaterLess = fldValue.Substring(0, 1);
            fldValue = fldValue.Substring(1, fldValue.Length - 1);
        }

        object oValue = null;
        switch (fldName)
        {
            case "rating":
                if (!double.TryParse(fldValue, out var d) || d < 0)
                    throw new Exception($"错误的查询格式：“{text}”");
                oValue = d;
                break;
            case "year":
                if (!int.TryParse(fldValue, out var i) || i < 1900)
                    throw new Exception($"错误的查询格式：“{text}”");
                oValue = i.ToString();
                break;
        }

        if (string.IsNullOrEmpty(greaterLess))
            greaterLess = "=";
        else switch (greaterLess)
        {
            case ">":
                greaterLess = ">=";
                break;
            case "<":
                greaterLess = "<=";
                break;
        }
        var pName = $"@p{command.Parameters.Count}";
        sb.Append($" and {fldName}{greaterLess}{pName}");
        command.Parameters.AddWithValue(pName, oValue);
        return true;
    }
    public SQLiteCommand BuildSearchCommand(string text,bool withFilter)
    {
        var command = new SQLiteCommand();
        var sb = new StringBuilder("select * from filelist_view where 1=1");

        //处理搜索关键词
        if (!string.IsNullOrEmpty(text))
        {
            var splits = text.Split(null);

            for (var i = 0; i < splits.Length; i++)
            {
                if (ProcessFilterFields(command, sb, splits[i])) continue;

                var pName = $"@p{command.Parameters.Count}";
                command.Parameters.AddWithValue(pName, $"%{splits[i]}%");
                sb.Append($" and (name like {pName} or keyname like{pName} or othername like {pName} or genres like {pName} or seecomment like {pName} or path like {pName} or doubanid like {pName})");

            }
        }

        if (!withFilter)
        {
            command.CommandText = sb.ToString();
            return command;
        }

        //过滤
        if (SeeLater) sb.Append(" and seelater=1");
        if (Watched && !NotWatched) sb.Append(" and seeflag=1");
        if (NotWatched && !Watched)
        {
            sb.Append(" and seeflag=0");
            if (HideSameSubject)
                sb.Append(" and  doubanid not in(select DISTINCT doubanid from tb_file where seeflag=1 and doubanid<>'')");
        }

        if (NotWatched)
        {
            sb.Append(" and seenowant=0");
            if (HideSameSubject)
                sb.Append(" and  doubanid not in(select DISTINCT doubanid from tb_file where seenowant=1 and doubanid<>'')");
        }

        if (HasDoubanId && !NoDoubanId)
        {
            sb.Append(" and doubanid<>0");
        }

        if (!HasDoubanId && NoDoubanId)
        {
            sb.Append(" and (doubanid=0 or doubanid is null)");

        }

        //限制条数
        var limitClause = $" limit {RecordsLimit}";

        //最近观看特殊处理
        if (Recent)
        {
            sb.Insert(0, "select * from (");
            sb.Append(" order by CreationTime desc");
            sb.Append(limitClause);
            sb.Append(")");
        }

        //排序
        var ordered = false;
        if (OrderRatingDesc.HasValue && OrderRatingDesc.Value)
        {
            sb.Append(" order by rating desc");
            ordered = true;
        }
        else if (OrderRatingDesc.HasValue && !OrderRatingDesc.Value)
        {
            sb.Append(" order by rating asc");
            ordered = true;
        }

        if (OrderYearDesc.HasValue && OrderYearDesc.Value)
            sb.Append(ordered ? ",year desc" : " order by year desc");
        else if (OrderYearDesc.HasValue && !OrderYearDesc.Value)
            sb.Append(ordered ? ",year asc" : " order by year asc");

        if (!Recent)
            sb.Append(limitClause);



        Debug.WriteLine(sb.ToString());

        command.CommandText = sb.ToString();
        return command;
    }
}