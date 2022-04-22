using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace _0DayDownRar
{
    public class WinRar
    {
        private readonly string _winRarPath;
        private readonly string _workPath;
        private const string ZeroDownPwd = "0daydown";

        public WinRar(string winRarPath, string workPath)
        {
            _winRarPath = winRarPath;
            _workPath = workPath;
        }

        private static readonly Dictionary<int, string> WinRarError = new Dictionary<int, string>()
        {
            { 1, " 警告。发生非致命错误" },
            { 2, " 发生致命错误。" },
            { 3, "无效校验和。数据损坏。 " },
            { 4, " 尝试修改一个 锁定的压缩文件。 " },
            { 5, " 写错误。 " },
            { 6, "文件打开错误。 " },
            { 7, "错误命令行选项。 " },
            { 8, "内存不足。 " },
            { 9, "文件创建错误。 " },
            { 10, " 没有找到与指定的掩码和选项匹配的文件。 " },
            { 11, " 密码错误。 " },
            { 255, " 用户中断。 " }
        };

        public (int, string) TestFile(string fileName)
        {
            var cmd = $" t -r -pC3ED2E2B-D14F-4FF0-8EC3-A6142A5AE2F7 -inul -ibck \"{fileName}\"";
            var (exitCode,message,_) = RunWinExe(Path.Combine(_winRarPath,"winrar.exe"), cmd);
            return (exitCode, message);
        }

        public string[] ListContent(string fileName)
        {
            var cmd = $" lb -p{ZeroDownPwd} -v \"{fileName}\"";
            var (exitCode, message, output) = RunWinExe(Path.Combine(_winRarPath, "unrar.exe"), cmd);
            if (exitCode!=0) throw new Exception(message);
            Debug.WriteLine(output);
            //return Regex.Split(output, "\r\n|\r|\n");
            return output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public (int,string) ExtractFile(string fileName, string extractDir)
        {
            var cmd = $"x -IBCK -o+ -y -inul -p{ZeroDownPwd} \"{fileName}\"  \"{extractDir}\"";//背景解压，覆盖文件不确认,禁止显示错误
            var (exitCode, message, _) = RunWinExe(Path.Combine(_winRarPath, "winrar.exe"), cmd);
            return (exitCode, message);
        }

        public (int, string) CompressFolder(string fileName, string extractDir)
        {
            var cmd = $"a -r -rr3p -v333m -vn -ep1 -df -t \"{fileName}\"  \"{extractDir}\"";
            var (exitCode, message, _) = RunWinExe(Path.Combine(_winRarPath, "rar.exe"), cmd);
            return (exitCode, message);
        }

        public void DeleteRarFiles(string fileName)
        {
            var name = Path.GetFileName(fileName);

            var match = Regex.Match(name, @"\.part\d*.rar",RegexOptions.IgnoreCase);
            if(!match.Success)
            {
                File.Delete(fileName);
                return;
            }
            var dir = Path.GetDirectoryName(fileName);
            if (string.IsNullOrEmpty(dir))
                throw new Exception("路径非法。");
            var s = name.Replace(match.Groups[0].Value, @"\.part\d*.rar");
            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                if(Regex.Match(Path.GetFileName(file),s).Success)
                    File.Delete(file);
            }


        }

        private (int,string,string) RunWinExe(string exe,string cmd)
        {
            Debug.WriteLine($"{exe} {cmd}");
            var procStartInfo = new ProcessStartInfo(exe, cmd)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = _workPath
            };
            var proc = new Process { StartInfo = procStartInfo };
            proc.Start();
            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            //var result = proc.StandardError.ReadToEnd();
            var errCode = WinRarError.ContainsKey(proc.ExitCode) ? WinRarError[proc.ExitCode] : "未知错误";
            return proc.ExitCode != 0 ? (proc.ExitCode,errCode,output) : (proc.ExitCode, "",output);
        }

        public static string RarBaseName(string fileName)
        {
            var name = Path.GetFileName(fileName);

            var match = Regex.Match(name, @"\.part\d*.rar",RegexOptions.IgnoreCase);
            return !match.Success ? Path.GetFileNameWithoutExtension(fileName) : name.Replace(match.Groups[0].Value, "");
        }
    }
}
