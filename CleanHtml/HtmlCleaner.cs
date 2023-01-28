using ImageMagick.Formats;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CleanHtml
{
    internal class HtmlCleaner
    {
        private readonly string _htmlFileName;
        private string _path;

        public HtmlCleaner(string htmlFileName)
        {
            _htmlFileName = htmlFileName;
            _path = Path.GetDirectoryName(htmlFileName);

        }


        public void Clean(FormOption.ActionType action,bool convertWebP)
        {
            switch (action)
            {
                case FormOption.ActionType.CleanImageLazyLoad:
                    CleanImageLazyLoad(_htmlFileName, convertWebP);
                    break;

                case FormOption.ActionType.ExtractEmbeddedImage:
                    ExtractBase64Image(_htmlFileName);
                    break;
            }
        }

        private void CleanImageLazyLoad(string htmlFile, bool convertWebp)
        {
            var content = File.ReadAllText(htmlFile);

            content = ClearImageLazy(content);

            if (convertWebp)
                content = SearchConvertWebP(content);

            File.WriteAllText(htmlFile, content);
        }

        private string ClearImageLazy(string content)
        {
            //code project
            content = Regex.Replace(content, @"data-src=[^\s]+\s", "");
            content = Regex.Replace(content, "data-srcset=\"[^\"]*\"", "");
            content = Regex.Replace(content, "lazyautosizes", "");
            content = Regex.Replace(content, @"\s+lazyloaded", "");


            //learnopencv
            content = Regex.Replace(content, "data-lazy-srcset=\"[^\"]*\"", "");
            content = Regex.Replace(content, "srcset=\"[^\"]*\"", "");

            content = Regex.Replace(content, @"data-lazy-src=[^\s]+[\s\>]", "");
            

            content = Regex.Replace(content, "data-lazy-sizes=\"[^\"]*\"", "");
            content = Regex.Replace(content, @"data-lazy-sizes=[^\s\>]*", "");


            content = Regex.Replace(content, @"data-ll-status=""?[^\s""\>]*", "");

            content = Regex.Replace(content, "decoding=async", "");
            content = Regex.Replace(content, "srcset", "");


            return content;
        }

        private bool ConvertWebP(string webPFile,out string format)
        {
            format = string.Empty;
            try
            {
                var filePath = Path.GetDirectoryName(webPFile);
                var fileName = Path.GetFileNameWithoutExtension(webPFile);




                using var image = new MagickImage(webPFile);
                if (image.AnimationDelay == 0)
                {
                    var defines = new JpegWriteDefines()
                    {
                        DctMethod = JpegDctMethod.Slow,
                        OptimizeCoding = false
                    };

                    var jpegFile = Path.Combine(filePath, $"{fileName}.jpeg");

                    image.Write(jpegFile, defines);
                    format = "jpeg";
                }
                else
                {
                    using var images = new MagickImageCollection(webPFile);
                    var gifFile = Path.Combine(filePath, $"{fileName}.gif");

                    images.Write(gifFile);
                    format = "gif";
                }


            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }


        private bool ConvertWebPStream(byte[] webPFile, string fileName, out string format)
        {
            format = string.Empty;
            try
            {


                using var image = new MagickImage(webPFile);
                if (image.AnimationDelay == 0)
                {
                    var defines = new JpegWriteDefines()
                    {
                        DctMethod = JpegDctMethod.Slow,
                        OptimizeCoding = false
                    };

                    var jpegFile = Path.Combine(_path, $"{fileName}.jpeg");

                    image.Write(jpegFile, defines);
                    format = "jpeg";
                }
                else
                {
                    using var images = new MagickImageCollection(webPFile);
                    var gifFile = Path.Combine(_path, $"{fileName}.gif");

                    images.Write(gifFile);
                    format = "gif";
                }


            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private class WebPConvertItem
        {
            public string FileName { get; set; }
            public string Format { get; set; }

            public string Replace
            {
                get
                {
                    var index = FileName.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
                    var s = FileName.Substring(0, index);
                    var s1 = s + "." + Format;
                    return s1;
                }
            }

        }

        private string SearchConvertWebP(string content)
        {
            var converted = new List<WebPConvertItem>();

            var match = Regex.Match(content, @"\<img[^\>\<]*src=([^\s]+webp)", RegexOptions.IgnoreCase);
            while (match.Success)
            {
                if (match.Groups.Count < 2)
                    goto next;

                var webpFile = Path.Combine(_path, match.Groups[1].Value);
                if (!File.Exists(webpFile))
                    goto next;

                if (!ConvertWebP(webpFile,out var format))
                    goto next;

                converted.Add(new WebPConvertItem {FileName = match.Groups[1].Value,Format = format});

            next:
                match = match.NextMatch();

            }

            var sb = new StringBuilder(content);
            foreach (var convert in converted)
            {
                sb.Replace(convert.FileName, convert.Replace);

            }

            return sb.ToString();
        }


        private  void ExtractBase64Image(string htmlFile)
        {
            var content = File.ReadAllText(htmlFile);

            content = ClearImageLazy(content);


            var sb = new StringBuilder(content);

            var filePath = Path.GetDirectoryName(htmlFile);
            if (!Directory.Exists(filePath)) return;

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

                        if (string.Compare(ext, "webp",CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0)
                        {
                            //处理webP
                            if(!ConvertWebPStream(data,guid,out var format))
                                continue;
                            img = $"{guid}.{format}";
                        }
                        else
                        {
                            File.WriteAllBytes(Path.Combine(filePath, img), data);
                        }


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

            var newFile = Path.Combine(filePath, Path.GetFileNameWithoutExtension(htmlFile) + "_1" + Path.GetExtension(htmlFile));

            Debug.WriteLine(newFile);

            File.WriteAllText(newFile, sb.ToString());

        }



    }
}
