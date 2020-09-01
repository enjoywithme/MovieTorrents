﻿using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace mySharedLib
{
    public static class Utility
    {


        //标准化文件名
        public static string MakeValidFileName(this string name)
        {
            var invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }

        //延迟执行某个操作
        //https://stackoverflow.com/questions/10458118/wait-one-second-in-running-program
        //Note Forms.Timer and Timer() have similar implementations. 
        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new System.Windows.Forms.Timer();;
            timer.Tick += delegate

            {
                timer.Stop();
                action.Invoke();
            };

            timer.Interval = millisecond;
            timer.Start();
        }

        public static T GetSetting<T>(string key, T defaultValue = default(T)) where T : IConvertible
        {
            var val = ConfigurationManager.AppSettings[key] ?? "";
            T result = defaultValue;
            if (string.IsNullOrEmpty(val)) return result;
            T typeDefault = default(T);
            if (typeof(T) == typeof(string))
            {
                typeDefault = (T)(object)string.Empty;
            }
            result = (T)Convert.ChangeType(val, typeDefault.GetTypeCode());
            return result;
        }
    }

    
}