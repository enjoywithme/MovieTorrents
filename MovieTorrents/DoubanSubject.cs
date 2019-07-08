using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieTorrents
{
    public class DoubanSubject
    {


        public string id { get; set; }
        public string title { get; set; }
        public string sub_title { get; set; }
        public string name { get; set; }
        public string othername { get; set; }
        public string type { get; set; }
        public string year { get; set; }
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
        public string img_local { get {
                if (!string.IsNullOrEmpty(_img_local)) return _img_local;
                TryToDownloadSubjectImg();
                return _img_local;
            } }

        private static CookieContainer cookieContainer;
        //<script type="application/ld+json"></script>
        //regex: <script type="application\/ld\+json">([\s\S]*?)<\/script>    https://www.regextester.com/93588
        private static Regex SubjectScriptJsonMatch = new Regex("<script type=\"application\\/ld\\+json\">([\\s\\S]*?)<\\/script>");

        private void TryToDownloadSubjectImg()
        {
            var filename = Path.GetFileName(img_url);
            if (string.IsNullOrEmpty(filename)) return;
            var filefullname = FormMain.CurrentPath + "\\temp\\" + filename;

            using (WebClient client = new WebClient())
            {
                var uri = new Uri(img_url);

                try
                {
                    client.DownloadFile(uri, filefullname);
                    _img_local = filefullname;
                }
                catch(Exception ex)
                {

                }
            }
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string lpszUrl, string lpszCookieName, StringBuilder lpszCookieData, ref int lpdwSize, int dwFlags, IntPtr lpReserved);

        const int ERROR_INSUFFICIENT_BUFFER = 122;

        const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        public static string GetCookies(string uri)
        {
            StringBuilder buffer;
            string result;
            int bufferLength;
            int flags;

            bufferLength = 1024;
            buffer = new StringBuilder(bufferLength);

            flags = INTERNET_COOKIE_HTTPONLY;

            if (InternetGetCookieEx(uri, null, buffer, ref bufferLength, flags, IntPtr.Zero))
            {
                result = buffer.ToString();
            }
            else
            {
                result = null;

                var r = Marshal.GetLastWin32Error();
                if (r == ERROR_INSUFFICIENT_BUFFER)
                {
                    buffer.Length = bufferLength;

                    if (InternetGetCookieEx(uri, null, buffer, ref bufferLength, flags, IntPtr.Zero))
                    {
                        result = buffer.ToString();
                    }
                }
            }

            return result;
        }

        private static void BuildCookies()
        {
            if(cookieContainer==null)
                cookieContainer = new CookieContainer();
            var url = "https://movie.douban.com";
            var cookieData = GetCookies(url);
            if (cookieData != null)
                cookieContainer.SetCookies(new Uri(url), cookieData.Replace("; ", ","));
        }

        public static List<DoubanSubject> SearchSuggest(string text)
        {
            var list = new List<DoubanSubject>();

            var q = Uri.EscapeDataString(text);
            var sUrl = $"https://movie.douban.com/j/subject_suggest?q={q}";
            var uri = new Uri(sUrl);


            BuildCookies();
            
            var jsonText = string.Empty;
#if !DEBUG
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.CookieContainer = cookieContainer;
                request.Method = "GET";
                request.Accept = "application/json; charset=utf-8";
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    jsonText = sr.ReadToEnd();
                    Debug.WriteLine(jsonText);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
#else
            jsonText = File.ReadAllText("douban_suggest_sbuject.json");

#endif


            if (string.IsNullOrEmpty(jsonText)) return list;


            try
            {
                var subjects = JArray.Parse(jsonText).ToList();
                foreach (var jobject in subjects)
                {
                    var subject = new DoubanSubject
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

            }

            return list;
        }

        public static List<DoubanSubject> SearchSubject(string text)
        {
            var list = new List<DoubanSubject>();

            //TODO:使用内嵌浏览器访问页面抓取

            return list;
        }

        public bool TryQueryDetail(out string msg)
        {
            msg = string.Empty;

            var sUrl = $"https://movie.douban.com/subject/{id}/";
            var uri = new Uri(sUrl);


            BuildCookies();

            var html = string.Empty;

#if !DEBUG
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.CookieContainer = cookieContainer;
                request.Method = "GET";
                var response = (HttpWebResponse)request.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    html = sr.ReadToEnd();
                    Debug.WriteLine(html);
                    File.WriteAllText("d:\\sample_subject.txt", html);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
#else

            html = File.ReadAllText("sample_subject.html");
#endif

            if (string.IsNullOrEmpty(html)) return false;
            //又名:</span> 星际启示录(港) / 星际效应(台) / 星际空间 / 星际之间 / 星际远航 / 星际 / Flora's Letter<br/>
            var match = Regex.Match(html, "又名:<\\/span>([\\s\\S]*?)<br\\/>");
            if (match.Success) othername = match.Groups[1].Value;

            match = SubjectScriptJsonMatch.Match(html);
            if (!match.Success) return false;
            html = match.Groups[1].Value;
            //html = html.Replace("<script type=\"application/ld+json\">","");//<script type="application/ld+json">
            //html = html.Replace("</script>", "");

            
            //Debug.Print(html);

            try
            {
                var jo = JObject.Parse(html);
                name = (string)jo["name"];
                year = string.Empty;
                if(DateTime.TryParse((string)jo["datePublished"],out var d)){
                    year = d.Year.ToString();
                }
                genres =string.Join(" ",jo["genre"].Select(t => (string)t).ToList());
                rating = (string) jo["aggregateRating"]["ratingValue"];
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }

            return true;
        }
    }
}
