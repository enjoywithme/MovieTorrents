using System;
using System.Collections.Generic;
using System.Text;

namespace FindSimilarNameFolder
{
    public class FolderItem
    {
        public string FolderName { get; private set; }
        public string FolderPath { get; private set; }
        public Boolean Checked { get; set; }

        public FolderItem(string folderName, string folderPath)
        {
            FolderPath = folderPath;
            FolderName = folderName;
            Checked = false;
        }

        public static Single Similarity(FolderItem src, FolderItem dest)
        {
            Single similarity;
            LevenshteinDistance(src.FolderName, dest.FolderName, out similarity);
            return similarity*100;
        }

        private static Int32 LevenshteinDistance(String source, String target, out Single similarity, Boolean isCaseSensitive = false)
        {
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target))
                {
                    similarity = 1;
                    return 0;
                }
                similarity = 0;
                return target.Length;
            }
            if (String.IsNullOrEmpty(target))
            {
                similarity = 0;
                return source.Length;
            }

            String from, to;
            if (isCaseSensitive)
            {   // 大小写敏感  
                from = source;
                to = target;
            }
            else
            {   // 大小写无关  
                from = source.ToLower();
                to = target.ToLower();
            }

            // 初始化  
            var m = from.Length;
            var n = to.Length;
            var h = new Int32[m + 1, n + 1];
            for (var i = 0; i <= m; i++) h[i, 0] = i;  // 注意：初始化[0,0]  
            for (var j = 1; j <= n; j++) h[0, j] = j;

            // 迭代  
            for (var i = 1; i <= m; i++)
            {
                var si = from[i - 1];
                for (var j = 1; j <= n; j++)
                {   // 删除（deletion） 插入（insertion） 替换（substitution）  
                    if (si == to[j - 1])
                        h[i, j] = h[i - 1, j - 1];
                    else
                        h[i, j] = Math.Min(h[i - 1, j - 1], Math.Min(h[i - 1, j], h[i, j - 1])) + 1;
                }
            }

            // 计算相似度  
            var maxLength = Math.Max(m, n);   // 两字符串的最大长度  
            similarity = ((Single)(maxLength - h[m, n])) / maxLength;

            return h[m, n];    // 编辑距离  
        }  
    }

    public class SimilarGroup
    {
        private readonly List<FolderItem> _items=new List<FolderItem>();
        public List<FolderItem> Items { get { return _items; } }

        public float Similarity { get; private set; }

        public SimilarGroup()
        {
            Similarity = 100;
        }

        public void Add(FolderItem item,float similarity)
        {
            _items.Add(item);
            if (Similarity > similarity) Similarity = similarity;
        }

        public void Add(FolderItem item)
        {
            _items.Add(item);
        }
    }

}
