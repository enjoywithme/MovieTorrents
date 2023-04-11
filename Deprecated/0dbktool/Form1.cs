using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace _0dbktool
{
    public partial class Form1 : Form
    {
        FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowser.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("The folder cannot be empty", "Warning", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            try
            {
                ArrayList suffixs =new ArrayList();
                suffixs.Add(".RAR");
                suffixs.Add(".EPUB");
                suffixs.Add(".PDF");

                DirectoryInfo drInfo = new DirectoryInfo(textBox1.Text);
                FileInfo[] files = drInfo.GetFiles();
                int i = 0;
                foreach (FileInfo file in files)
                {
                    string fileext = file.Extension.ToUpper();
                    if(suffixs.Contains(fileext))
                    {
                        string dirname = file.Directory + "\\" + file.Name;
                        dirname = dirname.Substring(0, dirname.Length - fileext.Length);
                        try
                        {
                            if (!Directory.Exists(dirname)) Directory.CreateDirectory(dirname);
                            file.MoveTo(dirname + "\\" + file.Name);
                            i++;

                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                }

                MessageBox.Show(string.Format("{0} files were processed!", i));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowser.SelectedPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = folderBrowser.SelectedPath;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox2.Text)||string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("The source and dest folder cannot be empty", "Warning", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            try
            {
                DirectoryInfo drInfo = new DirectoryInfo(textBox2.Text);
                DirectoryInfo[] directorys = drInfo.GetDirectories();
                int i = 0;
                foreach (DirectoryInfo directory in directorys)
                {
                    string[] s = directory.Name.Split('.');
                    if (s.Length < 2) continue;

                    int year = 0;
                    if (!int.TryParse(s[s.Length - 1], out year)) continue;
                    if (year < 1900 || year > 2100) continue;

                    int month = ParseMonth(s[s.Length - 2]);
                    if (month == 0) continue;

                    //创建文件夹
                    string dstfolder = textBox3.Text + "\\" + year.ToString();
                    try
                    {
                        if (!Directory.Exists(dstfolder)) Directory.CreateDirectory(dstfolder);
                        int days = DateTime.DaysInMonth(year, month);
                        dstfolder = string.Format("{0}\\{1:00}01-{2:00}{3:00}", dstfolder, month, month, days);
                        if (!Directory.Exists(dstfolder)) Directory.CreateDirectory(dstfolder);
                    }
                    catch (Exception exception)
                    {

                        MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //移动文件夹
                    directory.MoveTo(dstfolder+"\\"+directory.Name);

                    //
                    i++;
                }
                MessageBox.Show(string.Format("{0} folders were moved!", i));


            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }

        private int ParseMonth(string str)
        {
            switch (str.ToLower())
            {
                case "jan":
                    return 1;
                case "feb":
                    return 2;
                case "mar":
                    return 3;
                case "apr":
                    return 4;
                case "may":
                    return 5;
                case "jun":
                    return 6;
                case "jul":
                    return 7;
                case "aug":
                    return 8;
                case "sep":
                    return 9;
                case "oct":
                    return 10;
                case "nov":
                    return 11;
                case "dec":
                    return 12;
            }
            return 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox3.Text = textBox2.Text= textBox1.Text = AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
