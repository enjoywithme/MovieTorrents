﻿using System.Diagnostics;
using System.Globalization;
using System.Text;
using MyPageViewer.PoCo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyPageViewer.Model
{
    public class MyPageDocument:IDisposable
    {
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string GuId { get; set; }
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if(_title==value) return;
                _title = value;
                SetModified(true);
            }
        }

        private string _originUrl;

        public string OriginalUrl
        {
            get => _originUrl;
            set
            {
                if(_originUrl==value) return;
                _originUrl = value;
                SetModified(true);
            }
        }

        private int _rate;

        public int Rate
        {
            get => _rate;
            set
            {
                if(_rate==value) return;
                _rate = value;
                SetModified(true);
            }
        }

        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 文件已经被删除
        /// </summary>
        public bool Deleted { get; private set; }

        private bool _manifestChanged;
        public bool IsModified { get; private set; }
        public IList<string> Tags { get; set; }

        /// <summary>
        /// 解压后的临时根目录
        /// </summary>
        public string DocTempPath { get; private set; }
        /// <summary>
        /// 解压后的临时index.html
        /// </summary>
        public string TempIndexPath { get; private set; }

        public string TempAttachmentsPath => Path.Combine(DocTempPath, "Attachments");


        public static MyPageDocument NewFromArgs(string[] args)
        {
            if (args == null || args.Length == 0) return null;

            return (from arg in args 
                where File.Exists(arg) && Path.GetExtension(arg).ToLower() == ".piz" 
                select new MyPageDocument(arg)).FirstOrDefault();
        }

        public MyPageDocument(string filePath)
        {
            var poCo = MyPageDb.Instance.FindFilePath(filePath);
            FilePath = filePath;
            if (poCo != null)
            {
                _title = poCo.Title;
                FileSize = poCo.FileSize;
                CreatedDate = poCo.DtCreated;
                ModifiedDate = poCo.DtModified;
                return;
            }

            if(!File.Exists(filePath)) return;
            var fi = new FileInfo(filePath);
            FileSize = fi.Length;
            CreatedDate = fi.CreationTime;
            ModifiedDate = fi.LastWriteTime;

        }

        /// <summary>
        /// 解压到临时目录
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ExtractToTemp(out string message)
        {
            try
            {
                if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
                    throw new Exception("页文件不存在！");

                if (string.IsNullOrEmpty(GuId))
                    GuId = Guid.NewGuid().ToString();

                DocTempPath = Path.Combine(MyPageSettings.Instance.TempPath, GuId);
                if (Directory.Exists(DocTempPath))
                    Directory.Delete(DocTempPath,true);
                Directory.CreateDirectory(DocTempPath);

                System.IO.Compression.ZipFile.ExtractToDirectory(FilePath, DocTempPath);

                var fileName = Path.Combine(DocTempPath, "index.html");
                if(File.Exists(fileName))
                    TempIndexPath = fileName;

                fileName = Path.Combine(DocTempPath, "manifest.json");
                if (File.Exists(fileName))
                {
                    var jo = JObject.Parse(File.ReadAllText(fileName));
                    _title = (string) jo["title"];
                    _originUrl = (string)jo["originalUrl"];
                    var rate = (string)jo["rate"];
                    if (int.TryParse(rate, out var d))
                        _rate = d;
                    var tags = jo.Value<JArray>("tags");
                    if(tags!= null)
                        Tags = tags.ToObject<List<string>>();
                }
            }
            catch (Exception e)
            {
                message = e.Message;
                return false;
            }

            message = string.Empty;
            return true;
        }

        private static readonly JsonSerializerSettings JsonSerializerSettings
            = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

        /// <summary>
        /// 从临时目录重新打包
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool RepackFromTemp(out string message)
        {
            try
            {
                //Save manifest
                if (_manifestChanged)
                {
                    var fileName = Path.Combine(DocTempPath, "manifest.json");
                    var jo = File.Exists(fileName) ? JObject.Parse(File.ReadAllText(fileName)) : new JObject();

                    jo["title"] = _title;
                    jo["originalUrl"] = OriginalUrl;
                    jo["rate"] = Rate;
                    if (Tags != null)
                        jo["tags"] = JToken.FromObject(Tags);

                    File.WriteAllText(fileName, JsonConvert.SerializeObject(jo, JsonSerializerSettings),Encoding.UTF8);
                }


                var tempZip = Path.Combine(MyPageSettings.Instance.TempPath, $"{GuId}.zip");
                if(File.Exists(tempZip))
                    File.Delete(tempZip); 
                System.IO.Compression.ZipFile.CreateFromDirectory(DocTempPath, tempZip);
                File.Move(tempZip,FilePath,true);

                _manifestChanged = false;
                IsModified = false;
            }
            catch (Exception e)
            {
                message = e.Message;
                return false;
            }
            message = string.Empty;

            return true;
        }

        /// <summary>
        /// 清洗HTML
        /// </summary>
        /// <returns></returns>
        public bool CleanHtml(out string message)
        {
            message = string.Empty;
            if(string.IsNullOrEmpty(TempIndexPath)) return false;
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(TempIndexPath);

                //清除延迟加载的图片
                var imageNodes = doc.DocumentNode.SelectNodes("//img")?.Where(t => t.Attributes["data-src"] != null)
                    .ToList();
                if (imageNodes != null)
                {
                    foreach (var node in imageNodes)
                    {
                        var imgSrc = node.Attributes["src"];
                        if (imgSrc == null) continue;
                        Debug.WriteLine(imgSrc.Value);
                        node.Attributes.RemoveAll();
                        node.Attributes.Add("src", imgSrc.Value);
                    }
                }
                

                //删除crossorigin=anonymous
                var linkNodes = doc.DocumentNode.SelectNodes("//link")?.Where(t => t.Attributes["crossorigin"] != null)
                    .ToList();
                if (linkNodes != null)
                {
                    foreach (var node in linkNodes)
                    {
                        node.Attributes.Remove("crossorigin");
                    }
                }
                

                doc.Save(TempIndexPath);
            }
            catch (Exception e)
            {
                message = e.Message;
                return false;
            }
            
            

            return true;
        }


        public bool AddTag(string tag)
        {
            Tags ??= new List<string>();
            if (Tags.Any(x=>string.Compare(x,tag,CultureInfo.InvariantCulture, CompareOptions.IgnoreCase)==0)) return false;

            Tags.Add(tag);
            SetModified(true);

            return true;
        }

        public void RemoveTag(string tag)
        {
            Tags?.Remove(tag);
            SetModified(true);
        }

        public void SetModified(bool manifest=false)
        {
            if (manifest) _manifestChanged = true;
            IsModified = true;
        }


        public PageDocumentPoCo ToPoCo()
        {
            return new PageDocumentPoCo()
            {
                Guid = GuId,
                Title = _title,
                FilePath = FilePath,
                Rate = _rate,
                OriginUrl = _originUrl,
                FileSize = FileSize
            };
        }

        public void Dispose()
        {
            if(Directory.Exists(DocTempPath))
                Directory.Delete(DocTempPath, true);
        }
        ~MyPageDocument(){
            Dispose();
        }

        public bool Delete(out string message)
        {
            message = null;
            if (!File.Exists(FilePath))
            {
                Deleted = true;
                return true;
            }
            try
            {
                File.Delete(FilePath);
                MyPageDb.Instance.DeleteDocument(ToPoCo());
                Deleted = true;
            }
            catch (Exception e)
            {
                message = e.Message;
                return false;
            }

            return true;
        }
    }
}
