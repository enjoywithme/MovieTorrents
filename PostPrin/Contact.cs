using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace PostPrin
{
    public class Contact
    {
        public static string INI_FILE_NAME = "contacts.ini";
        public string Name = "";

        private string[] Fields =
        {
            "SendName",
            "SendPlace",
            "SendCompany",
            "SendProvince",
            "SendCity",
            "SendZone",
            "SendAddress",
            "SendPhone",
            "SendMobile",

            "RecvName",
            "RecvPlace",
            "RecvCompany",
            "RecvAddress",
            "RecvProvince",
            "RecvCity",
            "RecvZone",
            "RecvPhone",
            "RecvMobile"
        };

        public Hashtable Values = new Hashtable();

        public Contact()
        {
            
        }
        public Contact(string name)
        {
            Name = name;
        }

        public static string INIFilePath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + Contact.INI_FILE_NAME;
        }

        public void WriteToIniFile()
        {
            if(string.IsNullOrEmpty(Name)) return;
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
                    string v = iniFile.IniReadValue(this.Name, field);
                    Values[field] = v;
                }
                catch (Exception)
                {

                }
            }
        }

        public static string [] ContactsFromIniFile()
        {
            IniFile iniFile = new IniFile(INIFilePath());
            return iniFile.GetSectionNames();
        }

        public static void DeleteContactFromIniFile(string name)
        {
            IniFile iniFile = new IniFile(INIFilePath());
            iniFile.DeleteSection(name);
        }
    }
}
