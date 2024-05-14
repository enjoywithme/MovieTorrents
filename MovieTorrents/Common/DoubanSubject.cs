using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MovieTorrents.Common
{
    public class DouBanSubject
    {


        public string id { get; set; }
        public string title { get; set; }
        public string sub_title { get; set; }
        //public string name { get; set; }
        public string othername { get; set; }
        public string type { get; set; }
        public string year { get; set; }
        public string zone { get; set; }
        public string casts { get; set; }
        public string directors { get; set; }
        public string genres { get; set; }
        public string rating { get; set;}
        public double Rating { get
            {
                if (string.IsNullOrEmpty(rating)) return 0;
                if (!double.TryParse(rating, out var d)) return 0;
                return d;
            } }
        public string img_url { get; set; }
        private string _img_local = string.Empty;

        public string img_local
        {
            get
            {
                if (!string.IsNullOrEmpty(_img_local)) return _img_local;
                TryToDownloadSubjectImg();
                return _img_local;
            }
            set => _img_local = value;
        }


        private bool _triedDetail=false;

        public DouBanSubject()
        {
            casts=string.Empty;
            directors = string.Empty;
            genres = string.Empty;
        }

        private void TryToDownloadSubjectImg()
        {
            if(!string.IsNullOrEmpty(_img_local) && File.Exists(_img_local)) return;

            var filename = Path.GetFileName(img_url);
            if (string.IsNullOrEmpty(filename)) return;
            var tempFileName = MyMtSettings.Instance.CurrentPath + "\\temp\\" + filename;

            using var client = new WebClient();
            var uri = new Uri(img_url);

            try
            {
                client.DownloadFile(uri, tempFileName);
                _img_local = tempFileName;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static List<DouBanSubject> SearchSuggest(string text,out string msg)
        {
            msg = string.Empty;

            var list = new List<DouBanSubject>();

            var q = Uri.EscapeDataString(text);
            var sUrl = $"https://movie.douban.com/j/subject_suggest?q={q}";
            var uri = new Uri(sUrl);


            
            var jsonText = string.Empty;
#if !LOCALTEST
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.Accept = "application/json; charset=utf-8";
                var response = (HttpWebResponse)request.GetResponse();
                using var sr = new StreamReader(response.GetResponseStream());
                jsonText = sr.ReadToEnd();
                Debug.WriteLine(jsonText);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                msg = e.Message;
            }
#else
            jsonText = File.ReadAllText(FormMain.CurrentPath + "\\temp\\douban_suggest_sbuject.json");

#endif


            if (string.IsNullOrEmpty(jsonText)) return list;


            try
            {
                var subjects = JArray.Parse(jsonText).ToList();
                foreach (var jobject in subjects)
                {
                    var subject = new DouBanSubject
                    {
                        id = (string)jobject["id"],
                        title = (string)jobject["title"],
                        sub_title = (string)jobject["sub_title"],
                        type = (string)jobject["type"],
                        year = (string)jobject["year"],
                        img_url = (string)jobject["img"]
                    };
                    list.Add(subject);

                    

                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return list;
        }


        public static List<DouBanSubject> SearchById(string text,out string msg)
        {
            var subjectId = string.Empty;
            var list = new List<DouBanSubject>();

            var match = Regex.Match(text, "https:\\/\\/movie.douban.com\\/subject\\/(\\d+)",
                RegexOptions.IgnoreCase);
            if (match.Success) subjectId = match.Groups[1].Value;
            else
            {
                match = Regex.Match(text, "(\\d+)");
                if (match.Success) subjectId = match.Groups[1].Value;

            }

            if (string.IsNullOrEmpty(subjectId))
                msg = "不正确的豆瓣ID";
            else
            {
                var subject = new DouBanSubject() { id = subjectId };
                subject.TryQueryDetail(out msg);
                //subject.title = subject.name;
                if(string.IsNullOrEmpty(subject.sub_title))
                    subject.sub_title = subject.othername;
                list.Add(subject);
            }

            return list;
        }
        public bool TryQueryDetail(out string msg)
        {
            msg = string.Empty;

            if (_triedDetail) return true;

            var sUrl = $"https://movie.douban.com/subject/{id}/";
            var uri = new Uri(sUrl);


            var html = string.Empty;

#if !LOCALTEST
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                var response = (HttpWebResponse)request.GetResponse();

                using var sr = new StreamReader(response.GetResponseStream());
                html = sr.ReadToEnd();
                //Debug.WriteLine(html);
#if DEBUG
                File.WriteAllText(MyMtSettings.Instance.CurrentPath + "\\temp\\sample_subject.txt", html);
#endif
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                msg = e.Message;
                return false;
            }
#else

            html = File.ReadAllText(FormMain.CurrentPath + "\\temp\\sample_subject_standard.html");
#endif

            if (string.IsNullOrEmpty(html)) return false;
            //又名:</span> 星际启示录(港) / 星际效应(台) / 星际空间 / 星际之间 / 星际远航 / 星际 / Flora's Letter<br/>
            var match = Regex.Match(html, "又名:<\\/span>([\\s\\S]*?)<br\\/>");
            if (match.Success) othername = match.Groups[1].Value;
            
            //<span class="year">(2014)</span>
            match = Regex.Match(html, "<span class=\"year\">\\((\\d{4})\\)<\\/span>");
            if (match.Success) year = match.Groups[1].Value;

            //<span class="pl">制片国家/地区:</span> 美国 / 英国 / 加拿大 / 冰岛<br/>
            match = Regex.Match(html, "地区:<\\/span>([\\s\\S]*?)<br\\/>");
            if (match.Success) zone = match.Groups[1].Value;

            // https://www.regextester.com/93588
            //<script type="application/ld+json"></script>
            match = Regex.Match(html, "<script type=\"application\\/ld\\+json\">([\\s\\S]*?)<\\/script>");
            if (!match.Success) return false;
            html = match.Groups[1].Value;
        
            //Debug.Print(html);

            try
            {
                var jo = JObject.Parse(html);
                if(string.IsNullOrEmpty(title))
                    title = (string)jo["name"];
                if (string.IsNullOrEmpty(img_url)) img_url = (string)jo["image"];

                var datePublished = (string)jo["datePublished"];
                if (!string.IsNullOrEmpty(datePublished) && DateTime.TryParse(datePublished,out var d)){
                    year = d.Year.ToString();
                }
                
                directors = string.Join("|", jo["director"].Select(t => (string)t["name"]).ToList());
                casts = string.Join("|", jo["actor"].Select(t => (string) t["name"]).ToList());
                genres =string.Join(" ",jo["genre"].Select(t => (string)t).ToList());
                rating = (string) jo["aggregateRating"]["ratingValue"];

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }

            _triedDetail = true;

            return true;
        }

        public static DouBanSubject InitFromPageHtml(string sourceUrl, string html)
        {
            var subject = new DouBanSubject();
            var match = Regex.Match(sourceUrl, @"movie.douban.com/subject/(\d+)");
            if (match.Success)
                subject.id = match.Groups[1].Value;
            else return null;

            //名称 <span property="v:itemreviewed">雷神 Thor</span>
            match = Regex.Match(html, @"""v:itemreviewed"">(.*?)</span>", RegexOptions.IgnoreCase);
            if (match.Success) subject.title = match.Groups[1].Value;

            //year <span class="year">(2011)</span>
            match = Regex.Match(html, @"class=""year"">\((.*?)\)</span>", RegexOptions.IgnoreCase);
            if (match.Success) subject.year = match.Groups[1].Value;

            //导演 <span class="pl">导演</span>: <span class="attrs"><a href="/celebrity/1036342/" rel="v:directedBy">肯尼思·布拉纳</a></span>
            match = Regex.Match(html,@"rel=""v:directedBy"">(.*?)</a>",  RegexOptions.IgnoreCase);
            subject.directors= match.GetAllText();

            //主演</span>: <span class="attrs"><span><a href="/celebrity/1021959/" rel="v:starring">克里斯·海姆斯沃斯</a> / </span><span><a href="/celebrity/1054454/" rel="v:starring">娜塔莉·波特曼</a> / </span><span><a href="/celebrity/1004596/" rel="v:starring">汤姆·希德勒斯顿</a> / </span><span><a href="/celebrity/1054434/" rel="v:starring">安东尼·霍普金斯</a> / </span><span><a href="/celebrity/1017918/" rel="v:starring">斯特兰·斯卡斯加德</a> / </span><span style="display: none;"><a href="/celebrity/1018976/" rel="v:starring">凯特·戴琳斯</a> / </span><span style="display: none;"><a href="/celebrity/1000083/" rel="v:starring">克拉克·格雷格</a> / </span><span style="display: none;"><a href="/celebrity/1022706/" rel="v:starring">科鲁姆·费奥瑞</a>
            match = Regex.Match(html, @"rel=""v:starring"">(.*?)</a>", RegexOptions.IgnoreCase);
            subject.directors = match.GetAllText();

            match = Regex.Match(html, @"v:genre"">(.*?)</span>", RegexOptions.IgnoreCase);
            subject.genres = match.GetAllText();

            match = Regex.Match(html, @"class=""pl"">制片国家/地区:</span>(.*?)<br>", RegexOptions.IgnoreCase);
            subject.zone = match.GetAllText();

            match = Regex.Match(html, @"class=""pl"">又名:</span>(.*?)<br>", RegexOptions.IgnoreCase);
            subject.othername = match.GetAllText();

            match = Regex.Match(html, @"rating_num"" property=""v:average"">(.*?)</strong>", RegexOptions.IgnoreCase);
            subject.rating = match.GetAllText();

            match = Regex.Match(html, @"class=""pl"">集数:</span>(.*?)<br>", RegexOptions.IgnoreCase);
            subject.type = match.Success ? "电视剧" : "电影";

            match = Regex.Match(html, @"<div id=""mainpic"" class="""">[\s\S]*?src=""(.*?)""[\s\S]*?</div>", RegexOptions.IgnoreCase);
            if(match.Success) subject.img_url = match.Groups[1].Value;

            subject.TryToDownloadSubjectImg();

            //subject.title = subject.name;
            if(string.IsNullOrEmpty(subject.sub_title))
                subject.sub_title = subject.othername;

            return subject;
        }


    }
}
