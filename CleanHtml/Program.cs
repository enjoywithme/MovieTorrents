using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CleanHtml
{
    internal class Program
    {

        static void Main(string[] args)
        {
#if DEBUG

            var htmlFile =
                "X:\\temp\\index.html";
#else
            if (args.Length == 0) return;

            var htmlFile = args[0];
#endif

            Debug.WriteLine(htmlFile);
            if (!File.Exists(htmlFile)) return;

            var formOption = new FormOption();
            var ret = formOption.ShowDialog();
            if (ret == DialogResult.Cancel) return;

            var cleaner = new HtmlCleaner(htmlFile);
            cleaner.Clean(formOption.Action,formOption.ConvertWebp);

            

        }





    }
}
