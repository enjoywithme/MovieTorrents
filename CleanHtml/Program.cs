using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CleanHtml
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) return;
            var htmlFile = args[0];
            Debug.WriteLine(htmlFile);
            if (!File.Exists(htmlFile)) return;

            var formOption = new FormOption();
            var ret = formOption.ShowDialog();
            if (ret == DialogResult.Cancel) return;

            switch (formOption.Action)
            {
                case FormOption.ActionType.CleanImageLazyLoad:
                    CleanImageLazyLoad(htmlFile);
                    break;

                case FormOption.ActionType.ExtractEmbeddedImage:
                    ExtractBase64Image(htmlFile);
                    break;
            }

        }

        private static void CleanImageLazyLoad(string htmlFile)
        {
            var content = File.ReadAllText(htmlFile);

            content = Regex.Replace(content, "data-src=[^\\s]*?\\s", "");
            content = Regex.Replace(content, "data-srcset=\".*?\"", "");

            File.WriteAllText(htmlFile, content);
        }

        private static void ExtractBase64Image(string htmlFile)
        {
            var content = File.ReadAllText(htmlFile);

            content = Regex.Replace(content, "data-src=[^\\s]*?\\s", "");
            content = Regex.Replace(content, "data-srcset=\".*?\"", "");

            var sb = new StringBuilder(content);

            var filePath = Path.GetDirectoryName(htmlFile);
            if(!Directory.Exists(filePath)) return;

            var match = Regex.Match(content, "\\<img[\\s]*src=\"data:image\\/([\\S]*);base64,([\\S]*)\"");
            var ret = false;
            while (match.Success)
            {
                if (match.Groups.Count >= 3)
                {
                    var ext = match.Groups[1].Value;
                    var base64 = match.Groups[2].Value;

                    var guid = Guid.NewGuid().ToString("D");
                    var img = $"{guid}.{ext}";

                    var data = Convert.FromBase64String(base64);
                    File.WriteAllBytes(Path.Combine(filePath,img),data);

                    sb.Replace($"data:image/{ext};base64,{base64}", img);

                    ret = true;

                }
                

                match = match.NextMatch();
            }

            if (!ret) return;

            var newFile = Path.Combine(filePath, Path.GetFileNameWithoutExtension(htmlFile) + "_1"+ Path.GetExtension(htmlFile));

            Debug.WriteLine(newFile);

            File.WriteAllText(newFile,sb.ToString());

        }
    }
}
