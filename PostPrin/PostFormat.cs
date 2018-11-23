using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Collections;


namespace PostPrin
{
    public class PostFormat
    {
        public static string DEDAULT_POSTFORMAT_NAME = "默认面单格式";
        public static string INI_FILE_NAME = "postformat.ini";
        public string Name = DEDAULT_POSTFORMAT_NAME;

        protected string[] Fields =
        {
            "Width",
            "Height",
            "OffsetX",
            "OffsetY",

            "SendNameX",
            "SendNameY", 
            "SendPlaceX",   
            "SendPlaceY", 
            "SendProvinceX",
            "SendProvinceY",
            "SendProvinceLen",
            "SendCityX",
            "SendCityY",
            "SendCityLen",
            "SendZoneX",
            "SendZoneY",
            "SendZoneLen",
            "SendCompanyX",   
            "SendCompanyY",  
            "SendAddress1X",      
            "SendAddress1Y",    
            "SendAddress1Len",   
            "SendAddress2X",  
            "SendAddress2Y",  
            "SendAddress2Len",
            "SendPhoneX",     
            "SendPhoneY",  
            "SendMobileX",  
            "SendmobileY",    

            "RecvNameX",     
            "RecvNameY",    
            "RecvNameLen",    
            "RecvPlaceX",    
            "RecvPlaceY", 
            "RecvProvinceX",
            "RecvProvinceY",
            "RecvProvinceLen",
            "RecvCityX",
            "RecvCityY",
            "RecvCityLen",
            "RecvZoneX",
            "RecvZoneY",
            "RecvZoneLen",
            "RecvCompanyX",   
            "RecvCompanyY",    
            "RecvAddress1X",   
            "RecvAddress1Y",  
            "RecvAddress1Len",  
            "RecvAddress2X",     
            "RecvAddress2Y",  
            "RecvAddress2Len",
            "RecvPhoneX",      
            "RecvPhoneY",     
            "RecvMobileX",  
            "RecvmobileY"

        };
        public Hashtable Values = new Hashtable();


        //发信人打印位置


        private PaperSize paperSize = null;
        private Boolean needInitialize = true;
        public Boolean Landscape = false;

        public PostFormat()
        {
            Initialize();
        }

        public PostFormat(string name)
        {
            Initialize();

            Name = name;
        }

        public PostFormat(string name, int width, int height)
        {
            Initialize();

            Name = name;
            Values["Width"] = width;
            Values["Height"] = height;

        }

        public PaperSize PaperSize
        {
            get
            {
                if (paperSize == null)
                {
                    //一般打印机只接受高比宽大的纸张，所以这里自动调换宽高，而打印的时候自动设置portait或者landscape
                    int pWidth = (int)Values["Width"];
                    int pHeight = (int)Values["Height"];
                    //if ((int)Values["Width"] > (int)Values["Height"])
                    //{
                    //    pWidth = (int)Values["Height"];
                    //    pHeight = (int)Values["Width"]; ;
                    //    Landscape = true;
                    //}

                    paperSize = new PaperSize(this.PaperName,
                        PrinterUnitConvert.Convert(pWidth * 10, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.ThousandthsOfAnInch) / 10,
                        PrinterUnitConvert.Convert(pHeight * 10, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.ThousandthsOfAnInch) / 10
                        );
                }
                return paperSize;
            }
        }
        public string PaperName { get { return string.Format("快递面单-{0}", Name); } }
        public Boolean IsDefault { get { return this.Name == DEDAULT_POSTFORMAT_NAME; } }

        public void Initialize()
        {
            //for (int i = 0; i < Fields.Length; i++)
            //{
            //    Values.Add(Fields[i], 0);
            //}
            Values["Width"] = 230;
            Values["Height"] = 127;
            Values["OffsetX"] = 0;
            Values["OffsetY"] = 0;

            Values["SendNameX"] = 35;
            Values["SendNameY"] = 30;
            Values["SendPlaceX"] = 80;
            Values["SendPlaceY"] = 30;
            Values["SendProvinceX"] = 0;
            Values["SendProvinceY"] = 0;
            Values["SendProvinceLen"] = 0;
            Values["SendCityX"] = 0;
            Values["SendCityY"] = 0;
            Values["SendCityLen"] = 0;
            Values["SendZoneX"] = 0;
            Values["SendZoneY"] = 0;
            Values["SendZoneLen"] = 0;
            Values["SendCompanyX"] = 35;
            Values["SendCompanyY"] = 35;
            Values["SendAddress1X"] = 20;
            Values["SendAddress1Y"] = 45;
            Values["SendAddress1Len"] = 80;
            Values["SendAddress2X"] = 20;
            Values["SendAddress2Y"] = 55;
            Values["SendAddress2Len"] = 80;
            Values["SendPhoneX"] = 48;
            Values["SendPhoneY"] = 60;
            Values["SendMobileX"] = 75;
            Values["SendmobileY"] = 60;
            //收信人打印位置
            Values["RecvNameX"] = 125;
            Values["RecvNameY"] = 30;
            Values["RecvNameLen"] = 0;
            Values["RecvPlaceX"] = 170;
            Values["RecvPlaceY"] = 30;
            Values["RecvProvinceX"] = 0;
            Values["RecvProvinceY"] = 0;
            Values["RecvProvinceLen"] = 0;
            Values["RecvCityX"] = 0;
            Values["RecvCityY"] = 0;
            Values["RecvCityLen"] = 0;
            Values["RecvZoneX"] = 0;
            Values["RecvZoneY"] = 0;
            Values["RecvZoneLen"] = 0;
            Values["RecvCompanyX"] = 125;
            Values["RecvCompanyY"] = 35;
            Values["RecvAddress1X"] = 110;
            Values["RecvAddress1Y"] = 45;
            Values["RecvAddress1Len"] = 80;
            Values["RecvAddress2X"] = 110;
            Values["RecvAddress2Y"] = 55;
            Values["RecvAddress2Len"] = 80;
            Values["RecvPhoneX"] = 138;
            Values["RecvPhoneY"] = 60;
            Values["RecvMobileX"] = 165;
            Values["RecvmobileY"] = 60;
        }

        public void AddPaperFormat(string printerName)
        {
            try
            {
                PrinterHelper.AddCustomPaperSize(printerName, this.PaperName, Landscape ? (int)Values["Height"] : (int)Values["Width"], Landscape ? (int)Values["Width"] : (int)Values["Height"]);

            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show(exception.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RestPaperSize()
        {
            paperSize = null;
        }

        public static string INIFilePath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + PostFormat.INI_FILE_NAME;
        }
        public void WriteToIniFile()
        {
            IniFile iniFile = new IniFile(INIFilePath());

            foreach (string field in Values.Keys)
            {
                iniFile.IniWriteValue(this.Name, field, Values[field].ToString());
            }
        }

        public void ReadFromIniFile()
        {
            IniFile iniFile = new IniFile(INIFilePath());
            for (int i = 0; i < Fields.Length; i++)
            {
                string field = Fields[i];
                try
                {
                    int v = Int32.Parse(iniFile.IniReadValue(this.Name, field));
                    Values[field] = v;
                }
                catch (Exception)
                {

                }
            }
        }

        public void DeleteFromIniFile()
        {
            IniFile iniFile = new IniFile(INIFilePath());
            iniFile.DeleteSection(this.Name);
        }
    }

    public class PostFormatCollection
    {
        protected Hashtable postFormats = new Hashtable();
        private static PostFormatCollection instance = null;
        public static PostFormatCollection Instance()
        {
            if (instance == null)
            {
                instance = new PostFormatCollection();
                PostFormat postFormat = new PostFormat();
                instance.postFormats.Add(postFormat.Name, postFormat);
            }
            return instance;
        }

        public static Hashtable PostFormats { get { return PostFormatCollection.Instance().postFormats; } }
        public static PostFormat DefaultPostFormat { get { return (PostFormat)PostFormatCollection.Instance().postFormats[PostFormat.DEDAULT_POSTFORMAT_NAME]; } }

        public static void AddPostFormat(PostFormat postFormat)
        {
            if (instance != null)
            {
                postFormat.WriteToIniFile();
                instance.postFormats.Add(postFormat.Name, postFormat);
            }
        }

        public static void DeletePostFormat(string postFormatName)
        {
            if (instance != null)
            {
                PostFormat postFormat = (PostFormat)instance.postFormats[postFormatName];
                postFormat.DeleteFromIniFile();
                instance.postFormats.Remove(postFormatName);
            }
        }

        public static void LoadPostFormatsFromIniFile()
        {
            IniFile iniFile = new IniFile(PostFormat.INIFilePath());
            string[] sections = iniFile.GetSectionNames();

            foreach (string section in sections)
            {
                section.Trim();
                if (string.IsNullOrEmpty(section)) continue;
                if (section != PostFormatCollection.DefaultPostFormat.Name)
                {
                    PostFormat postFormat = new PostFormat(section);
                    postFormat.ReadFromIniFile();
                    AddPostFormat(postFormat);
                }
                else
                {
                    PostFormat postFormat = PostFormatCollection.DefaultPostFormat;
                    postFormat.ReadFromIniFile();
                }
            }
        }
    }
}
