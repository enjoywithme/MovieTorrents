using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CleanHtmlLazyLoadingImg
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) return;
            var htmlFile = args[0];
            Debug.WriteLine(htmlFile);
            if (!File.Exists(htmlFile)) return;

            var content = File.ReadAllText(htmlFile);

            content = Regex.Replace(content, "data-src=[^\\s]*?\\s", "");
            content = Regex.Replace(content, "data-srcset=\".*?\"", "");

            File.WriteAllText(htmlFile,content);

        }
    }
}
