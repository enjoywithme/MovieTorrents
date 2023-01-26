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
#if DEBUG

            var htmlFile =
                "X:\\temp\\Building An Automated Image Annotation Tool_ PyOpenAnnotate (2023_1_26 20_39_47).html";
#else
            if (args.Length == 0) return;

            var htmlFile = args[0];
#endif


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

            content = ClearImageLazy(content);

            File.WriteAllText(htmlFile, content);
        }

        private static string ClearImageLazy(string content)
        {
            content = Regex.Replace(content, "data-src=[^\\s]*?\\s", "");
            content = Regex.Replace(content, "data-lazy-src=[^\\s]*?\\s", "");

            content = Regex.Replace(content, "data-srcset=\".*?\"", "");
            content = Regex.Replace(content, "data-lazy-srcset=\".*?\"", "");

            content = Regex.Replace(content, "data-lazy-sizes=\".*?\"", "");

            content = Regex.Replace(content, "data-ll-status=[^\\s]*?\\s", "");

            return content;
        }

        private static void ExtractBase64Image(string htmlFile)
        {
            var content = File.ReadAllText(htmlFile);

            content = ClearImageLazy(content);


            var sb = new StringBuilder(content);

            var filePath = Path.GetDirectoryName(htmlFile);
            if(!Directory.Exists(filePath)) return;

            //\<img[^\>\<]*src="?data:image\/([a-z]*);base64,([\S]+)\s+
            var match = Regex.Match(content, @"\<img[^\>\<]*src=""?data:image\/([a-z]*);base64,([^""\s]+)");
            var ret = false;
            var i = 0;
            while (match.Success)
            {
                i++;
                if (match.Groups.Count >= 3)
                {
                    try
                    {
                        var ext = match.Groups[1].Value;
                        var base64 = match.Groups[2].Value;

                        var guid = Guid.NewGuid().ToString("D");
                        var img = $"{guid}.{ext}";

                        var data = Convert.FromBase64String(base64);
                        File.WriteAllBytes(Path.Combine(filePath, img), data);

                        sb.Replace($"data:image/{ext};base64,{base64}", img);

                        ret = true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
#if DEBUG
                        File.WriteAllText(Path.Combine(filePath, $"{i}.txt"), match.Groups[2].Value);
#endif
                    }


                }
                

                match = match.NextMatch();
            }

            Debug.WriteLine(i.ToString());

            if (!ret) return;

            var newFile = Path.Combine(filePath, Path.GetFileNameWithoutExtension(htmlFile) + "_1"+ Path.GetExtension(htmlFile));

            Debug.WriteLine(newFile);

            File.WriteAllText(newFile,sb.ToString());

        }
    }
}
